using Application.Dtos.Role;
using Application.Dtos.UserClaim;

namespace Application.Dtos.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
        public bool Active { get; set; }

        public List<RoleDto> Roles { get; set; } = new();
        public List<UserClaimDto> UserClaims { get; set; } = new();
    }

    public class UpdateUserPasswordInput
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string RePassword { get; set; } = string.Empty;
    }
}