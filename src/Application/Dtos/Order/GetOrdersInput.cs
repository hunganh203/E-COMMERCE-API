using Application.DTOs.Pagination;

namespace Application.Dtos.Order
{
    public class GetOrdersInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
        public int Status { get; set; }
        public int SystemStatus { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetOrdersByCustomerInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
        public int Status { get; set; }
        public int SystemStatus { get; set; }
    }
}