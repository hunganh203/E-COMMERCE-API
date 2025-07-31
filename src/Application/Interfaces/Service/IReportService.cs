using Application.Dtos.Order;
using Application.Dtos.Product;
using Application.Dtos.Report;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IReportService
    {
        Task<ReportHighlight> GetHighlight(DateTime? date);

        Task<List<OrderDto>> GetGeneralReport(string keySearch, int status, DateTime? fDate, DateTime? tDate);

        Task<PagedResultDto<ProductDto>> GetProductReport(GetProductReport input);

        Task<GetRevenueReportOutput> GetRevenueReport(DateTime? date);
    }
}