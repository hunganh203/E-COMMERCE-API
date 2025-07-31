using Application.DTOs.Pagination;

namespace Application.Dtos.Menu
{
    public class GetMenuInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
        public int? PMenuId { get; set; }
         
    }
}