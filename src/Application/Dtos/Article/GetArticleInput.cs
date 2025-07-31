using Application.DTOs.Pagination;

namespace Application.Dtos.Article
{
    public class GetArticleInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
    }
}