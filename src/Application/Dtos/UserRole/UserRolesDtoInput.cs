namespace Application.Dtos.UserRole
{
    public class UserRolesDtoInput
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}