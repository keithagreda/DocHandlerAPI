using DocumentHandlerAPI.Models.Dtos;

namespace DocumentHandlerAPI.Services
{
    public interface IPdfGeneratorService
    {
        Task<ApiResponse<string>> GeneratePDFAsync(PdfGenerationRequest req);
    }
}