using Application.DTOs.Pagination;

namespace Application.Dtos.ProductDisplay
{
    public class GetProductDisplaysInput : PagedInputDto
    {
        public int Type { get; set; }
        public string? KeySearch { get; set; } = string.Empty;
    }
}