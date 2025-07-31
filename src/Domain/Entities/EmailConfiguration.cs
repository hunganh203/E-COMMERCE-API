using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("EmailConfiguration")]
    public class EmailConfiguration
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}