using Application.Constants;
using Application.Dtos.Order;
using Application.Dtos.Product;
using Application.Dtos.Report;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale},{UserRoleKey.RoleConst.InventoryManagement}")]
    public class ReportsController : BaseAuthAdminApiController
    {
        private readonly IReportService _reportService;

        public ReportsController(IUserService userService, IReportService reportService)
            : base(userService)
        {
            _reportService = reportService;
        }

        [HttpPost("highlight")]
        public async Task<AjaxResponse<ReportHighlight>> GetHighlight(ReportInput input)
        {
            try
            {
                var result = await this._reportService.GetHighlight(input.Date);
                return new AjaxResponse<ReportHighlight>(result);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<ReportHighlight>(new ErrorInfo(ex.Message));
            }
        }

        [Route("general")]
        [HttpPost]
        public async Task<AjaxResponse<List<OrderDto>>> GetGeneralReport(ReportInput input)
        {
            try
            {
                var result = await this._reportService.GetGeneralReport(input.KeySearch, input.Status, input.FinalDate, input.TzDate);
                return new AjaxResponse<List<OrderDto>>(result);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<List<OrderDto>>(new ErrorInfo(ex.Message));
            }
        }

        [Route("product-report")]
        [HttpPost]
        public async Task<AjaxResponse<PagedResultDto<ProductDto>>> GetProductReport(GetProductReport input)
        {
            try
            {
                var result = await this._reportService.GetProductReport(input);
                return new AjaxResponse<PagedResultDto<ProductDto>>(result);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<PagedResultDto<ProductDto>>(new ErrorInfo(ex.Message));
            }
        }

        [Route("revenue")]
        [HttpPost]
        public async Task<AjaxResponse<GetRevenueReportOutput>> GetRevenueReport(ReportInput input)
        {
            try
            {
                var result = await this._reportService.GetRevenueReport(input.Date);
                return new AjaxResponse<GetRevenueReportOutput>(result);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<GetRevenueReportOutput>(new ErrorInfo(ex.Message));
            }
        }
    }
}