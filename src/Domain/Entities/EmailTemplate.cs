using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("EmailTemplate")]
    public class EmailTemplate
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string CC { get; set; } = string.Empty;
        public string BCC { get; set; } = string.Empty;
        public string KeyGuide { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}