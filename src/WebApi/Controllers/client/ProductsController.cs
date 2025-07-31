using Application.Dtos;
using Application.Dtos.Product;
using Application.DTOs.Pagination;
using Application.Dtos.ProductDisplay;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    public class ProductsController : BaseNonAuthClientApiController
    {
        private readonly IProductService _productService;
        private readonly IProductDisplayService _productDisplayService;

        public ProductsController(IProductService productService, IProductDisplayService productDisplayService)
        {
            _productService = productService;
            _productDisplayService = productDisplayService;
        }

        [HttpGet("get-list")]
        public async Task<AjaxResponse<PagedResultDto<ProductDto>>> GetList([FromQuery] GetProductInput input)
        {
            var products = await _productService.GetAll(input);
            return new AjaxResponse<PagedResultDto<ProductDto>>(products);
        }

        [HttpGet("get-products-display")]
        public async Task<AjaxResponse<PagedResultDto<ProductDto>>> GetProductsDisplay([FromQuery] GetProductDisplaysInput input)
        {
            var products = await _productDisplayService.GetForClient(input);
            return new AjaxResponse<PagedResultDto<ProductDto>>(products);
        } 


        [HttpGet("{id}")]
        public async Task<AjaxResponse<ProductDto>> GetById(Guid id)
        {
            try
            {
                var product = await _productService.GetByIdForClient(id);
                return new AjaxResponse<ProductDto>(product);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ProductDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpGet("get-products-related")]
        public async Task<AjaxResponse<List<ProductDto>>> GetProductRelateds(Guid id)
        {
            try
            {
                var product = await _productService.GetProductRelateds(id);
                return new AjaxResponse<List<ProductDto>>(product);
            }
            catch (Exception e)
            {
                return new AjaxResponse<List<ProductDto>>(new ErrorInfo(e.Message));
            }
        }
    }
}