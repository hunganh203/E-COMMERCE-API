using Application.Constants;
using Application.Dtos.Order;
using Application.Dtos.Review;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    [Route("api/order/")]
    public class OrdersController : BaseAuthClientApiController
    {
        private readonly IOrderService _orderService;
        private readonly IReviewService _reviewService;

        public OrdersController(ICustomerService customerService, IOrderService orderService, IReviewService reviewService) : base(customerService)
        {
            _orderService = orderService;
            _reviewService = reviewService;
        }

        [HttpGet("{id}")]
        public async Task<AjaxResponse<OrderDto>> GetById(int id)
        {
            try
            {
                var order = await _orderService.GetByIdForCustomer(id, UserId.GetValueOrDefault());
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
                orderInput.CustomerId = UserId.GetValueOrDefault();
                orderInput.CreatedDate = DateTimeOffset.UtcNow;
                orderInput.CreatedBy = UserId.GetValueOrDefault();
                orderInput.SourceFrom = OrderConst.SourceFrom.CustomerOrder;

                var order = await _orderService.Insert(orderInput, UserId.GetValueOrDefault());
                return new AjaxResponse<OrderDto>(order);
            }
            catch (Exception e)
            {
                return new AjaxResponse<OrderDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("review")]
        public async Task<AjaxResponse> Review(CustomerReviewInput input)
        {
            try
            {
                await this._reviewService.ReviewByCustomer(input, UserId.GetValueOrDefault());
                return new AjaxResponse(true);
            }
            catch (Exception ex)
            {
                return new AjaxResponse(new ErrorInfo(ex.Message));
            }
        }

        //[HttpPost("get-orders")]
        //public async Task<AjaxResponse<PagedResultDto<OrderDto>>> GetOrders(GetOrdersInput input)
        //{
        //    try
        //    {
        //        var result = await _orderService.GetOrders(input);

        //        return new AjaxResponse<PagedResultDto<OrderDto>>(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return new AjaxResponse<PagedResultDto<OrderDto>>(new ErrorInfo(e.Message));
        //    }
        //}
    }
}