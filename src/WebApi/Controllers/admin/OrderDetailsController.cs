using Application.Constants;
using Application.Dtos.Order;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale},{UserRoleKey.RoleConst.InventoryManagement}")]
    public class OrderDetailsController : BaseAuthAdminApiController
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IUserService userService, IOrderDetailService orderDetailService) : base(userService)
        {
            _orderDetailService = orderDetailService;
        }

        //[HttpGet("get-list")]
        //public async Task<AjaxResponse<List<OrderDto>>> GetList()
        //{
        //    var orders = await _orderDetailService.GetAll();
        //    return new AjaxResponse<List<OrderDto>>(orders);
        //}

        [HttpGet("{id}")]
        public async Task<AjaxResponse<OrderDetailDto>> GetById(int id)
        {
            try
            {
                var orderDetail = await _orderDetailService.GetById(id);
                return new AjaxResponse<OrderDetailDto>(orderDetail);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("add")]
        public async Task<AjaxResponse<OrderDetailDto>> Add(OrderDetailDto orderDetailInput)
        {
            try
            {
                var orderDetail = await _orderDetailService.Insert(orderDetailInput);
                return new AjaxResponse<OrderDetailDto>(orderDetail);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<OrderDetailDto>> Update(int id, OrderDetailDto orderDetailInput)
        {
            try
            {
                await _orderDetailService.Update(id, orderDetailInput);
                return new AjaxResponse<OrderDetailDto>(orderDetailInput);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<AjaxResponse<OrderDetailDto>> Delete(int id)
        {
            try
            {
                await _orderDetailService.DeleteById(id);
                return new AjaxResponse<OrderDetailDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-quantity")]
        public async Task<AjaxResponse<OrderDetailDto>> UpdateQuantity(int orderDetailId, int quantity)
        {
            try
            {
                var result = await _orderDetailService.UpdateQuantity(orderDetailId, quantity);
                return new AjaxResponse<OrderDetailDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-stock-receiving-name")]
        public async Task<AjaxResponse<OrderDetailDto>> UpdateStockReceivingName(int orderDetailId, string? stockReceivingName)
        {
            try
            {
                var result = await _orderDetailService.UpdateStockReceivingName(orderDetailId, stockReceivingName?? string.Empty);
                return new AjaxResponse<OrderDetailDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-price-stock-receiving")]
        public async Task<AjaxResponse<OrderDetailDto>> UpdatePriceStockReceiving(int orderDetailId, double priceStockReceiving)
        {
            try
            {
                var result = await _orderDetailService.UpdatePriceStockReceiving(orderDetailId, priceStockReceiving);
                return new AjaxResponse<OrderDetailDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-status")]
        public async Task<AjaxResponse<UpdateOrderDetailStatusOutput>> UpdateStatus(int orderDetailId, int status)
        {
            try
            {
                var result = await _orderDetailService.UpdateStatus(orderDetailId, status);
                return new AjaxResponse<UpdateOrderDetailStatusOutput>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<UpdateOrderDetailStatusOutput>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-note")]
        public async Task<AjaxResponse<OrderDetailDto>> UpdateNote(int orderDetailId, string? note)
        {
            try
            {
                var result = await _orderDetailService.UpdateNote(orderDetailId, note ?? string.Empty);
                return new AjaxResponse<OrderDetailDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-checking-code")]
        public async Task<AjaxResponse<OrderDetailDto>> UpdateCheckingCode(int orderDetailId, string? checkingCode)
        {
            try
            {
                var result = await _orderDetailService.UpdateCheckingCode(orderDetailId, checkingCode ?? string.Empty);
                return new AjaxResponse<OrderDetailDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-price-product")]
        public async Task<AjaxResponse<OrderDetailDto>> UpdatePriceProduct(int orderDetailId, double productPrice, double productDiscountPrice)
        {
            try
            {
                var result = await _orderDetailService.UpdatePriceProduct(orderDetailId, productPrice, productDiscountPrice);
                return new AjaxResponse<OrderDetailDto>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDetailDto>(new ErrorInfo(e.Message));
            }
        }
    }
}