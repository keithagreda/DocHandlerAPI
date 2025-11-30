namespace DocumentHandlerAPI.Models.Dtos
{
    public class PdfGenerationRequest
    {
        public string Id { get; set; }
        public string Html { get; set; } = string.Empty;
        public string Css { get; set; } = string.Empty;
        public string? FileName { get; set; }
    }
}
