using Amazon.S3;
using Amazon.S3.Model;
using DocumentHandlerAPI.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;

namespace DocumentHandlerAPI.Services
{
    public class S3FileStorageService : IS3FileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<S3FileStorageService> _logger;

        public S3FileStorageService(
            IAmazonS3 s3Client,
            IConfiguration configuration,
            ILogger<S3FileStorageService> logger)
        {
            _s3Client = s3Client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> SavePdfAsync(byte[] pdfBytes, string fileName)
        {
            var bucketName = _configuration["AWS:S3:BucketName"];
            var key = $"documents/{DateTime.UtcNow:yyyy/MM/dd}/{fileName}";

            using var stream = new MemoryStream(pdfBytes);

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream,
                ContentType = "application/pdf",
                CannedACL = S3CannedACL.Private
            };

            var res = await _s3Client.PutObjectAsync(putRequest);

            if (res.HttpStatusCode == HttpStatusCode.OK ||
                res.HttpStatusCode == HttpStatusCode.NoContent)
            {
                // Success - S3 returns 200 OK or 204 No Content for successful uploads
                var url = $"https://{bucketName}.s3.amazonaws.com/{key}";
                _logger.LogInformation("PDF uploaded to S3: {Url}", url);
                return ApiResponse<string>.Success(url);
            }
            else
            {
                _logger.LogError("S3 upload failed with status code: {StatusCode}", res.HttpStatusCode);
                return ApiResponse<string>.Failure($"Failed to upload to S3. Status code: {res.HttpStatusCode}");
            }
        }

        public async Task<ApiResponse> DeletePdfAsync(string fileUrl)
        {
            try
            {
                var bucketName = _configuration["AWS:S3:BucketName"];

                // Extract the S3 key from URL
                var key = ExtractS3KeyFromUrl(fileUrl, bucketName);

                if (string.IsNullOrEmpty(key))
                {
                    _logger.LogWarning("Invalid S3 URL: {FileUrl}", fileUrl);
                    return ApiResponse.Failure($"Invalid S3 URL: {fileUrl}");
                }

                // Check if object exists
                var objectExists = await CheckObjectExistsAsync(bucketName, key);
                if (!objectExists)
                {
                    _logger.LogWarning("S3 object not found: {Key}", key);
                    return ApiResponse.Failure($"S3 object not found: {key}");
                }

                // Delete the object
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);

                _logger.LogInformation("PDF deleted from S3: {Key}", key);
                return ApiResponse.Success();
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "S3 error deleting PDF: {FileUrl}, StatusCode: {StatusCode}",
                    fileUrl, ex.StatusCode);
                return ApiResponse.Failure($"S3 error deleting PDF: {fileUrl}, StatusCode: {ex.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PDF from S3: {FileUrl}", fileUrl);
                return ApiResponse.Failure($"Error deleting PDF from S3: {fileUrl}");
            }
        }

        private string? ExtractS3KeyFromUrl(string fileUrl, string bucketName)
        {
            try
            {
                // Handle different S3 URL formats:
                // 1. https://bucket-name.s3.amazonaws.com/documents/2024/11/30/file.pdf
                // 2. https://bucket-name.s3.region.amazonaws.com/documents/2024/11/30/file.pdf
                // 3. https://s3.region.amazonaws.com/bucket-name/documents/2024/11/30/file.pdf

                var uri = new Uri(fileUrl);

                // Format 1 & 2: bucket-name.s3.*.amazonaws.com/key
                if (uri.Host.StartsWith($"{bucketName}.s3"))
                {
                    return uri.AbsolutePath.TrimStart('/');
                }

                // Format 3: s3.*.amazonaws.com/bucket-name/key
                if (uri.Host.StartsWith("s3.") && uri.AbsolutePath.StartsWith($"/{bucketName}/"))
                {
                    return uri.AbsolutePath.Replace($"/{bucketName}/", "");
                }

                _logger.LogWarning("Unable to extract S3 key from URL: {FileUrl}", fileUrl);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing S3 URL: {FileUrl}", fileUrl);
                return null;
            }
        }

        private async Task<bool> CheckObjectExistsAsync(string bucketName, string key)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                await _s3Client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}
