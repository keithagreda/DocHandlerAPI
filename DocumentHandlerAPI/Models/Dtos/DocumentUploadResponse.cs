namespace DocumentHandlerAPI.Models.Dtos
{
    public class DocumentUploadResponse
    {
        public Ulid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
