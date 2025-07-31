using Application.Constants;
using Application.Dtos.Product;
using Application.Dtos.ProductDisplay;
using Application.DTOs.Pagination;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Shared.Services
{
    public class ProductDisplayService : IProductDisplayService
    {
        private readonly IProductDisplayRepository _productDisplayRepository;
        private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        public ProductDisplayService(IProductDisplayRepository productDisplayRepository, IMapper mapper, IConfiguration configuration)
        {
            _productDisplayRepository = productDisplayRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<PagedResultDto<ProductDisplayDto>> GetAll(GetProductDisplaysInput input)
        {
            var productsQuery = _productDisplayRepository
                .AsQueryable()
                .Where(x => input.Type < 0 || x.Type == input.Type);

            if (!string.IsNullOrEmpty(input.KeySearch))
            {
                productsQuery = productsQuery.Include(x => x.Product)
                    .Where(x => (x.Product!.Name.Contains(input.KeySearch.Trim()) || x.Product.Code.Contains(input.KeySearch.Trim())));
            }

            var productDisplays = await productsQuery
                .Skip(input.SkipCount)
                .Include(x => x.Product)
                .Take(input.MaxResultCount)
                .ToListAsync();

            if (productDisplays is not { Count: > 0 })
            {
                var productResult = _mapper.Map<List<ProductDisplayDto>>(productDisplays);
                return new PagedResultDto<ProductDisplayDto>
                {
                    Items = productResult,
                    TotalCount = productResult.Count
                };
            }

            var productDisplayDtos = _mapper.Map<List<ProductDisplayDto>>(productDisplays);
            foreach (var product in productDisplayDtos)
            {
                product.Product.ImageUrl = !string.IsNullOrEmpty(product.Product.Image)
                    ? S3Path.GetS3Url(_configuration, product.Product.Image)
                    : "";
            }
            var result = _mapper.Map<List<ProductDisplayDto>>(productDisplayDtos);

            return new PagedResultDto<ProductDisplayDto>
            {
                Items = result,
                TotalCount = input.Type < 0 && string.IsNullOrEmpty(input.KeySearch)
                    ? _productDisplayRepository.Count()
                    : await productsQuery.CountAsync()
            };
        }

        public async Task<PagedResultDto<ProductDto>> GetForClient(GetProductDisplaysInput input)
        {
            var productsQuery = _productDisplayRepository
                .AsQueryable()
                .Where(x => input.Type < 0 || x.Type == input.Type);

            if (!string.IsNullOrEmpty(input.KeySearch))
            {
                productsQuery = productsQuery.Include(x => x.Product)
                    .Where(x => (x.Product!.Name.Contains(input.KeySearch.Trim()) || x.Product.Code.Contains(input.KeySearch.Trim())));
            }

            var productDisplays = await productsQuery
                .Skip(input.SkipCount)
                .Include(x => x.Product)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var products = productDisplays.Select(x => x.Product);

            if (productDisplays is not { Count: > 0 })
            {
                var productResult = _mapper.Map<List<ProductDto>>(products);
                return new PagedResultDto<ProductDto>
                {
                    Items = productResult,
                    TotalCount = productResult.Count
                };
            }

            var productDisplayDtos = _mapper.Map<List<ProductDisplayDto>>(productDisplays);
            foreach (var product in productDisplayDtos)
            {
                product.Product.ImageUrl = !string.IsNullOrEmpty(product.Product.Image)
                    ? S3Path.GetS3Url(_configuration, product.Product.Image)
                    : "";
            }
            var result = _mapper.Map<List<ProductDto>>(productDisplayDtos.Select(x => x.Product));

            return new PagedResultDto<ProductDto>
            {
                Items = result,
                TotalCount = input.Type < 0 && string.IsNullOrEmpty(input.KeySearch)
                    ? _productDisplayRepository.Count()
                    : await productsQuery.CountAsync()
            };
        }

        public async Task<ProductDisplayDto> GetById(Guid id)
        {
            var productDisplay = await _productDisplayRepository
                .AsQueryable()
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<ProductDisplayDto>(productDisplay);
        }

        public async Task<List<ProductDisplayDto>> Add(ProductDisplayAddDto input)
        {
            // check valid
            var listInvalid = await this._productDisplayRepository.AsQueryable()
                .Where(x => x.Type == input.Type && input.ProductIds.Any(y => x.ProductId == y))
                .Include(x => x.Product)
                .Select(x => $"{x.Product!.Code} - {x.Product.Name}")
                .ToListAsync() ?? new List<string>();

            if (listInvalid.Count > 0)
            {
                throw new Exception(JsonConvert.SerializeObject(new
                {
                    message = "Sản phẩm đã có trong danh sách",
                    products = listInvalid
                }));
            }

            var productDisplays = input.ProductIds.Select(productId =>
            {
                var productDisplay = new ProductDisplay
                {
                    Id = Guid.NewGuid(),
                    Type = input.Type,
                    ProductId = productId,
                    Metadata = input.Metadata ?? "",
                };
                return productDisplay;
            });

            var result = await this._productDisplayRepository.AddRangeAsync(productDisplays);
            return _mapper.Map<List<ProductDisplayDto>>(result);
        }

        public async Task<ProductDisplayDto> Update(ProductDisplayDto input)
        {
            var invalid = await this._productDisplayRepository.AsQueryable()
                .AnyAsync(x => x.ProductId == input.ProductId && x.Type == input.Type);
            if (invalid)
            {
                throw new Exception(CustomResponseMessage.ProductHasExisted);
            }

            var productDisplay = await _productDisplayRepository
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            productDisplay.Type = input.Type;
            productDisplay.ProductId = input.ProductId;
            productDisplay.Metadata = input.Metadata;

            await this._productDisplayRepository.UpdateAsync(productDisplay);
            return _mapper.Map<ProductDisplayDto>(productDisplay);
        }

        public async Task<bool> Delete(Guid id)
        {
            var productDisplay = await _productDisplayRepository
                .FirstOrDefaultAsync(x => x.Id == id);

            await _productDisplayRepository.DeleteAsync(productDisplay);

            return true;
        }

        public async Task<bool> Delete(List<Guid> ids)
        {
            var productDisplay = await _productDisplayRepository
                .AsQueryable().Where(x => ids.Any(id => id == x.Id)).ToListAsync();

            await _productDisplayRepository.DeleteRangeAsync(productDisplay);

            return true;
        }
    }
}