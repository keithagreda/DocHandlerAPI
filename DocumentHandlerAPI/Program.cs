using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using DocumentHandlerAPI.Data;
using DocumentHandlerAPI.Interceptor;
using DocumentHandlerAPI.Interfaces;
using DocumentHandlerAPI.Models.Dtos;
using DocumentHandlerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        )));
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // Very important to compress even on HTTPS
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest; // or CompressionLevel.Optimal
});

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IS3FileStorageService, S3FileStorageService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var accessKey = config["AWS:Credentials:AccessKey"];
    var secretKey = config["AWS:Credentials:SecretKey"];
    var region = config["AWS:Region"];

    if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
    {
        throw new InvalidOperationException("AWS credentials not configured!");
    }

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    var s3Config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(region)
    };

    return new AmazonS3Client(credentials, s3Config);
});

builder.Services.AddScoped<AuditInterceptor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/api/documents/generate-pdf", async (
    [FromBody] PdfGenerationRequest request,
    [FromServices] IPdfGeneratorService pdfService) =>
{
    var result = await pdfService.GeneratePDFAsync(request);

    return result.IsSuccess
        ? Results.Ok(result)
        : Results.StatusCode((int)result.StatusCode);
})
.WithName("GeneratePDF");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
