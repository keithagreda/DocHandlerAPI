namespace DocumentHandlerAPI.Models.Dtos
{
    public class PdfGenerationRequest
    {
        public string Html { get; set; } = string.Empty;
        public string Css { get; set; } = string.Empty;
        public string Title { get; set; }
        public string Description { get; set; }
        //public string? FileName { get; set; }
    }
}
