using Application.DTOs.Pagination;

namespace Application.Dtos.Customer
{
    public class GetCustomerInput : PagedInputDto
    {
        public string? KeySearch { get; set; } = string.Empty;
    }
}