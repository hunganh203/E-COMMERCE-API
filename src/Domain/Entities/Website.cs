using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Website")]
    public class Website
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Favicon { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Youtube { get; set; } = string.Empty;
        public string Copyright { get; set; } = string.Empty;
    }
}