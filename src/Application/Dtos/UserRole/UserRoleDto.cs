using Application.Dtos.Role;
using Application.Dtos.User;

namespace Application.Dtos.UserRole
{
    public class UserRoleDto
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public UserDto User { get; set; } = new();

        public RoleDto Role { get; set; } = new();
    }
}