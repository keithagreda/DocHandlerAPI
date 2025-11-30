namespace DocumentHandlerAPI.Interfaces
{
    public interface IS3FileStorageService
    {
        Task<ApiResponse<string>> SavePdfAsync(byte[] pdfBytes, string fileName);
        Task<ApiResponse> DeletePdfAsync(string fileUrl);
    }
}