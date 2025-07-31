using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string NormalizedUserName { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Avatar { get; set; } = string.Empty;

        [MaxLength(256)]
        public string NormalizedEmail { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public bool? IsVerifiedPhone { get; set; }

        public bool? IsVerifiedEmail { get; set; }

        [MaxLength(256)]
        public string PasswordResetToken { get; set; } = string.Empty;

        public DateTimeOffset PasswordResetExpiration { get; set; }
    }
}