using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("EmailRegistration")]
    public class EmailRegistration
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
    }
}