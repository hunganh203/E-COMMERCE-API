using Application.DTOs.Pagination;

namespace Application.Dtos.User
{
    public class GetUserInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
    }
}