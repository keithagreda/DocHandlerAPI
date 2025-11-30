using DocumentHandlerAPI.Models.Dtos;

namespace DocumentHandlerAPI.Interfaces
{
    public interface IDocumentService
    {
        Task<ApiResponse<Ulid>> CreateDoc(CreateDocumentDto dto);
    }
}