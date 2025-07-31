using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("EmailSignUp")]
    public class EmailSignUp
    {
        [Key]
        public string Email { get; set; } = string.Empty;

        public string OTP { get; set; } = string.Empty;
    }
}