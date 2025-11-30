using DocumentHandlerAPI.Models.Dtos;

namespace DocumentHandlerAPI.Interfaces
{
    public interface IPdfGeneratorService
    {
        Task<ApiResponse<string>> GeneratePDFAsync(PdfGenerationRequest req);
    }
}