using Application.Constants;
using Application.Dtos.Customer;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale}")]
    public class CustomersController : BaseAuthAdminApiController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(IUserService userService, ICustomerService customerService) : base(userService)
        {
            _customerService = customerService;
        }

        [HttpGet("get-list")]
        public async Task<AjaxResponse<PagedResultDto<CustomerDto>>> GetList([FromQuery] GetCustomerInput input)
        {
            try
            {
                var customers = await _customerService.GetAll(input);
                return new AjaxResponse<PagedResultDto<CustomerDto>>(customers);
            }
            catch (Exception e)
            {
                return new AjaxResponse<PagedResultDto<CustomerDto>>(new ErrorInfo(e.Message));
            }
        }

        [HttpGet("get-customers-for-select")]
        public async Task<AjaxResponse<List<CustomerDto>>> GetCustomersForSelect(string? keySearch, int pageSize = 10)
        {
            var customerDtos = await _customerService.GetCustomersForSelect(keySearch, pageSize);
            return new AjaxResponse<List<CustomerDto>>(customerDtos);
        }

        [HttpGet("{id}")]
        public async Task<AjaxResponse<CustomerDto>> GetById(int id)
        {
            try
            {
                var customer = await _customerService.GetById(id);
                return new AjaxResponse<CustomerDto>(customer);
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("add")]
        public async Task<AjaxResponse<CustomerDto>> Add(CustomerDto customerInput)
        {
            try
            {
                var customer = await _customerService.Insert(customerInput);
                return new AjaxResponse<CustomerDto>(customer);
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<CustomerDto>> Update(int id, CustomerDto input)
        {
            try
            {
                var customer = await _customerService.UpdateFull(id, input);
                return new AjaxResponse<CustomerDto>(customer);
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<AjaxResponse<CustomerDto>> Delete(int id)
        {
            try
            {
                await _customerService.DeleteById(id);
                return new AjaxResponse<CustomerDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<CustomerDto>(new ErrorInfo(e.Message));
            }
        }
    }
}