using Application.Constants;
using Application.Dtos.Product;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale}")]
    public class ProductsController : BaseAuthAdminApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IUserService userService, IProductService productService) : base(userService)
        {
            _productService = productService;
        }

        [HttpGet("get-list")]
        public async Task<AjaxResponse<PagedResultDto<ProductDto>>> GetList([FromQuery] GetProductInput input)
        {
            var products = await _productService.GetAllForAdmin(input);
            return new AjaxResponse<PagedResultDto<ProductDto>>(products);
        }

        [HttpGet("get-products-for-select")]
        public async Task<AjaxResponse<List<ProductDto>>> GetProductsForSelect([FromQuery] GetProductsForSelect input)
        {
            var products = await _productService.GetProductsForSelect(input);
            return new AjaxResponse<List<ProductDto>>(products);
        }

        [HttpGet("{id}")]
        public async Task<AjaxResponse<ProductDto>> GetById(Guid id)
        {
            try
            {
                var product = await _productService.GetById(id);
                return new AjaxResponse<ProductDto>(product);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("add")]
        public async Task<AjaxResponse<ProductDto>> Add(ProductDto productInput)
        {
            try
            {
                var product = await _productService.Insert(productInput, this.UserId.GetValueOrDefault());
                return new AjaxResponse<ProductDto>(product);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<ProductDto>> Update(ProductDto productInput)
        {
            try
            {
                await _productService.Update(productInput, this.UserId.GetValueOrDefault());
                return new AjaxResponse<ProductDto>(productInput);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<AjaxResponse<ProductDto>> Delete(Guid id)
        {
            try
            {
                await _productService.DeleteById(id);
                return new AjaxResponse<ProductDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDto>(new ErrorInfo(e.Message));
            }
        }
    }
}