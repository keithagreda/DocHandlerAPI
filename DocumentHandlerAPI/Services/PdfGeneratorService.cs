using Amazon.S3;
using DocumentHandlerAPI.Interfaces;
using DocumentHandlerAPI.Models.Dtos;
using System.Text;

namespace DocumentHandlerAPI.Services
{
    public class PdfGeneratorService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PdfGeneratorService> _logger;
        private readonly IS3FileStorageService _s3FileStorageService;
        public PdfGeneratorService(ILogger<PdfGeneratorService> logger, IConfiguration configuration, HttpClient httpClient, IS3FileStorageService s3FileStorageService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _s3FileStorageService = s3FileStorageService;
        }

        public async Task<ApiResponse<string>> GeneratePDFAsync(PdfGenerationRequest req)
        {
            try
            {
                // 1. Prepare HTML for Gotenberg
                var fullHtml = PrepareHtmlForGotenberg(req.Html, req.Css);

                // 2. Call Gotenberg to generate PDF
                var pdfBytes = await ConvertHtmlToPdfAsync(fullHtml);

                // 3. Upload to S3
                var fileName = req.FileName ?? $"{req.Id}.pdf";
                var s3Url = await _s3FileStorageService.SavePdfAsync(pdfBytes, fileName);

                if (!s3Url.IsSuccess)
                {
                    return ApiResponse<string>.Failure(s3Url.Error.Message, s3Url.StatusCode);
                }

                // 4. Save to database (implement based on your entity model)
                // await SavePdfRecordAsync(request.EntityId, s3Url, fileName);

                return ApiResponse<string>.Success(s3Url.Data);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<byte[]> ConvertHtmlToPdfAsync(string html)
        {
            var gotenbergUrl = _configuration["Gotenberg:Url"] ?? "http://localhost:3000";
            var endpoint = $"{gotenbergUrl}/forms/chromium/convert/html";

            using var content = new MultipartFormDataContent();

            // Add HTML file
            var htmlContent = new StringContent(html, Encoding.UTF8, "text/html");
            content.Add(htmlContent, "files", "index.html");

            // Optional: Add Gotenberg options
            content.Add(new StringContent("A4"), "paperWidth");
            content.Add(new StringContent("A4"), "paperHeight");
            content.Add(new StringContent("0.4"), "marginTop");
            content.Add(new StringContent("0.4"), "marginBottom");
            content.Add(new StringContent("0.4"), "marginLeft");
            content.Add(new StringContent("0.4"), "marginRight");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        private string PrepareHtmlForGotenberg(string html, string css)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        {css}
                    </style>
                </head>
                <body>
                    {html}
                </body>
                </html>";
        }
    }
}
