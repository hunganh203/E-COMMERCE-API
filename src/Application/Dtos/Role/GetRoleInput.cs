using Application.DTOs.Pagination;

namespace Application.Dtos.Role
{
    public class GetRoleInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
    }
}