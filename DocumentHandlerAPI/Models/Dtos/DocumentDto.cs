namespace DocumentHandlerAPI.Models.Dtos
{
    public class DocumentDto
    {
        public Ulid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string DocumentType { get; set; }
    }
}
