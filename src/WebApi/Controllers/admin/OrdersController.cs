using Application.Constants;
using Application.Dtos;
using Application.Dtos.Order;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale},{UserRoleKey.RoleConst.InventoryManagement}")]
    public class OrdersController : BaseAuthAdminApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IUserService userService, IOrderService orderService) : base(userService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<AjaxResponse<OrderDto>> GetById(int id)
        {
            try
            {
                var order = await _orderService.GetById(id);
                return new AjaxResponse<OrderDto>(order);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("add")]
        public async Task<AjaxResponse<OrderDto>> Add(OrderDto orderInput)
        {
            try
            {
                var order = await _orderService.Insert(orderInput, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(order);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<OrderDto>> Update(int id, OrderDto orderInput)
        {
            try
            {
                await _orderService.Update(id, orderInput, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(orderInput);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin}")]
        [HttpPost("delete")]
        public async Task<AjaxResponse<OrderDto>> Delete(int id)
        {
            try
            {
                await _orderService.Delete(id);
                return new AjaxResponse<OrderDto>(true);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-deposit")]
        public async Task<AjaxResponse<OrderDto>> UpdateDeposit(int id, float value)
        {
            try
            {
                var result = await _orderService.UpdateDeposit(id, value, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-discount-price")]
        public async Task<AjaxResponse<OrderDto>> UpdateDiscountPrice(int id, double value)
        {
            try
            {
                var result = await _orderService.UpdateDiscountPrice(id, value, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-address")]
        public async Task<AjaxResponse<OrderDto>> UpdateAddress(int id, string value)
        {
            try
            {
                var result = await _orderService.UpdateAddress(id, value, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-shipping-info")]
        public async Task<AjaxResponse<OrderDto>> UpdateShippingInfo(int id, OrderShippingInfo value)
        {
            try
            {
                var result = await _orderService.UpdateShippingInfo(id, value, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-phone-number")]
        public async Task<AjaxResponse<OrderDto>> UpdatePhoneNumber(int id, string value)
        {
            try
            {
                var result = await _orderService.UpdatePhoneNumber(id, value, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-note")]
        public async Task<AjaxResponse<OrderDto>> UpdateNote(int id, string value)
        {
            try
            {
                var result = await _orderService.UpdateNote(id, value, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-stock-note")]
        public async Task<AjaxResponse<OrderDto>> UpdateStockNote(int id, string value)
        {
            try
            {
                var result = await _orderService.UpdateStockNote(id, value, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-status")]
        public async Task<AjaxResponse<OrderDto>> UpdateStatus(int id, int systemStatus, int status)
        {
            try
            {
                var order = await _orderService.ChangeStatus(id, systemStatus, status);
                return new AjaxResponse<OrderDto>(order);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("generate-invoice-of-order")]
        public async Task<AjaxResponse<FileStorageDto>> GenerateInvoiceOfOrder(int id)
        {
            try
            {
                var result = await _orderService.GenerateInvoiceOfOrder(id);
                return new AjaxResponse<FileStorageDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<FileStorageDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("get-orders")]
        public async Task<AjaxResponse<PagedResultDto<OrderDto>>> GetOrders(GetOrdersInput input)
        {
            try
            {
                var result = await _orderService.GetOrders(input);

                return new AjaxResponse<PagedResultDto<OrderDto>>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<PagedResultDto<OrderDto>>(new ErrorInfo(e.Message));
            }
        }
    }
}