using System.Security.Authentication;
using Application.Dtos.Customer;
using Application.Dtos.Order;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    /// <summary>
    /// User
    /// </summary>
    ///
    [Route("api/customer/")]
    public class CustomerController : BaseAuthClientApiController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerController> _logger;
        private readonly IVerificationService _verificationService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService,
            IConfiguration configuration,
            ILogger<CustomerController> logger,
            IVerificationService verificationService, IOrderService orderService) : base(customerService)
        {
            _customerService = customerService;
            _configuration = configuration;
            _logger = logger;
            _verificationService = verificationService;
            _orderService = orderService;
        }

        /// <summary>
        /// Get user info
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-user-info")]
        public async Task<AjaxResponse<CustomerDto>> GetUserInfo()
        {
            if (!UserId.HasValue)
            {
                throw new AuthenticationException("You are not authorized");
            }

            try
            {
                var user = await CustomerService.GetUserByIdAsync(UserId.Value);
                return new AjaxResponse<CustomerDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpGet("get-orders")]
        public async Task<AjaxResponse<PagedResultDto<OrderDto>>> GetOrders([FromQuery] GetOrdersByCustomerInput input)
        {
            if (!UserId.HasValue)
            {
                throw new AuthenticationException("You are not authorized");
            }
            try
            {
                var user = await _orderService.GetOrdersForCustomer(input, UserId.Value);
                return new AjaxResponse<PagedResultDto<OrderDto>>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<PagedResultDto<OrderDto>>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-profile")]
        public async Task<AjaxResponse<CustomerDto>> UpdateProfile(CustomerDto input)
        {
            if (!UserId.HasValue)
            {
                throw new AuthenticationException("You are not authorized");
            }
            try
            {
                var user = await _customerService.Update(UserId.Value, input);
                return new AjaxResponse<CustomerDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-password")]
        public async Task<AjaxResponse<CustomerDto>> UpdatePassword(UpdateCustomerPasswordInput input)
        {
            if (!UserId.HasValue)
            {
                throw new AuthenticationException("You are not authorized");
            }
            try
            {
                var user = await _customerService.UpdatePassword(UserId.Value, input);
                return new AjaxResponse<CustomerDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(e.Message));
            }
        }
    }
}