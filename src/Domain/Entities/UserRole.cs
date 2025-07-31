using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("UserRole")]
    public class UserRole
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}