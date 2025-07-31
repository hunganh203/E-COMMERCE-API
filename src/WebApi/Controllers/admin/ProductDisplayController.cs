using Application.Constants;
using Application.Dtos.ProductDisplay;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale}")]
    [Route("/product-display/")]
    public class ProductDisplayController : BaseAuthAdminApiController
    {
        private readonly IProductDisplayService _productDisplayService;

        public ProductDisplayController(IUserService userService, IProductDisplayService productDisplayService) : base(userService)
        {
            _productDisplayService = productDisplayService;
        }

        [HttpGet("get-list")]
        public async Task<AjaxResponse<PagedResultDto<ProductDisplayDto>>> GetList([FromQuery] GetProductDisplaysInput input)
        {
            var products = await _productDisplayService.GetAll(input);
            return new AjaxResponse<PagedResultDto<ProductDisplayDto>>(products);
        }

        [HttpGet("{id}")]
        public async Task<AjaxResponse<ProductDisplayDto>> GetById(Guid id)
        {
            try
            {
                var product = await _productDisplayService.GetById(id);
                return new AjaxResponse<ProductDisplayDto>(product);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDisplayDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("add")]
        public async Task<AjaxResponse<List<ProductDisplayDto>>> Add(ProductDisplayAddDto productInput)
        {
            try
            {
                var product = await _productDisplayService.Add(productInput);
                return new AjaxResponse<List<ProductDisplayDto>>(product);
            }
            catch (Exception e)
            {
                return new AjaxResponse<List<ProductDisplayDto>>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<ProductDisplayDto>> Update(ProductDisplayDto productInput)
        {
            try
            {
                await _productDisplayService.Update(productInput);
                return new AjaxResponse<ProductDisplayDto>(productInput);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDisplayDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<AjaxResponse<ProductDisplayDto>> Delete(List<Guid> ids)
        {
            try
            {
                await _productDisplayService.Delete(ids);
                return new AjaxResponse<ProductDisplayDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDisplayDto>(new ErrorInfo(e.Message));
            }
        }
    }
}