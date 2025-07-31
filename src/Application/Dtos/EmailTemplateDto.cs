namespace Application.Dtos
{
    public class EmailTemplateDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Cc { get; set; } = string.Empty;
        public string Bcc { get; set; } = string.Empty;
        public string KeyGuide { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}