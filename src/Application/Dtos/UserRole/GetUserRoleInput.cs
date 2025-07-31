using Application.DTOs.Pagination;

namespace Application.Dtos.UserRole
{
    public class GetUserRoleInput : PagedInputDto
    {
        public string KeySearch { get; set; } = string.Empty;
    }
}