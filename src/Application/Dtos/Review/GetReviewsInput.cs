using Application.DTOs.Pagination;

namespace Application.Dtos.Review
{
    public class GetReviewsInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
        public int Status { get; set; }
    }

    public class GetByProductInput : PagedInputDto
    {
        public Guid ProductId { get; set; }
    }
}