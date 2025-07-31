using Application.DTOs.Pagination;

namespace Application.Dtos.Product
{
    public class GetProductInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public int? Status { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string? SortBy { get; set; }
    }

    public class GetProductsForSelect
    {
        public string? KeySearch { get; set; } = string.Empty;

        public List<Guid> ExcludeIds { get; set; } = new();
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 0;
    }

    public class GetProductReport : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; }
    }
}