using Amazon.Runtime.Internal;
using DocumentHandlerAPI.Data;
using DocumentHandlerAPI.Interfaces;
using DocumentHandlerAPI.Models;
using DocumentHandlerAPI.Models.Dtos;
using System.Net.Http;
using System.Text;

namespace DocumentHandlerAPI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IS3FileStorageService _s3FileStorageService;
        private readonly ILogger<DocumentService> _logger;
        public DocumentService(AppDbContext dbContext, IS3FileStorageService s3FileStorageService, ILogger<DocumentService> logger)
        {
            _dbContext = dbContext;
            _s3FileStorageService = s3FileStorageService;
            _logger = logger;
        }

        public async Task<ApiResponse<Ulid>> CreateDoc(CreateDocumentDto dto)
        {
            try
            {
                Document document = new Document
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    DocumentType = dto.DocumentType,
                    URL = dto.URL,
                };

                await _dbContext.AddAsync(document);
                _logger.LogInformation("Successfully created a new document!");
                return ApiResponse<Ulid>.Success(document.Id);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Something went wrong while creating a new document");
                return ApiResponse<Ulid>.Failure($"Error creating document type {dto.DocumentType} - {dto.Title}");
            }
        }

    }
}
