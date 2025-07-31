using Application.Constants;
using Application.Dtos;
using Application.Dtos.Product;
using Application.DTOs.Pagination;
using Application.Extensions;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IProductAttributeRepository _productAttributeRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductRelatedRepository _productRelatedRepository;
        private readonly IAttributeRepository _attributeRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProductService(IProductRepository productRepository,
            IMapper mapper, IMenuRepository menuRepository, IProductAttributeRepository productAttributeRepository, IProductImageRepository productImageRepository, IProductRelatedRepository productRelatedRepository, IConfiguration configuration, IAttributeRepository attributeRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _menuRepository = menuRepository;
            _productAttributeRepository = productAttributeRepository;
            _productImageRepository = productImageRepository;
            _productRelatedRepository = productRelatedRepository;
            _configuration = configuration;
            _attributeRepository = attributeRepository;
        }

        public async Task DeleteById(Guid key)
        {
            await this._productRepository.DeleteById(key);
        }

        public async Task<List<ProductDto>> GetProductsForSelect(GetProductsForSelect input)
        {
            var products = await _productRepository
                .AsQueryable()
                .OrderByDescending(x => x.UpdatedDate)
                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Name.Contains(input.KeySearch.Trim()) || x.Code.Contains(input.KeySearch.Trim())))
                .Where(x => input.ExcludeIds.Count == 0 || !input.ExcludeIds.Any(y => x.Id == y))
                .Skip(input.PageIndex)
                .Take(input.PageSize)
                .Include(x => x.Menu)
                .Include(x => x.ProductAttributes).ThenInclude(p => p.Attribute)
                .ToListAsync();

            if (products is not { Count: > 0 })
                return _mapper.Map<List<ProductDto>>(products);

            var productsDto = _mapper.Map<List<ProductDto>>(products);
            this.RestructureAttribute(productsDto);

            foreach (var product in productsDto)
            {
                product.ImageUrl = !string.IsNullOrEmpty(product.Image)
                    ? S3Path.GetS3Url(_configuration, product.Image)
                    : "";
            }
            return productsDto;
        }

        public async Task<PagedResultDto<ProductDto>> GetAll(GetProductInput input)
        {
            if (input.MaxPrice.HasValue && !input.MinPrice.HasValue)
            {
                input.MinPrice = 0;
            }
            if (!input.MaxPrice.HasValue && input.MinPrice.HasValue)
            {
                input.MinPrice = double.MaxValue;
            }

            var productsQuery = _productRepository
                .AsQueryable()
                .OrderBy(x => x.Index)
                .Where(x => !input.CategoryId.HasValue || x.MenuId == input.CategoryId)
                .Where(x => !input.Status.HasValue || x.Status == input.Status)

                .Where(x => !(input.MinPrice.HasValue && input.MaxPrice.HasValue)
                            || (x.DiscountPrice >= input.MinPrice.GetValueOrDefault() && x.DiscountPrice <= input.MaxPrice.GetValueOrDefault()))

                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Name.Contains(input.KeySearch.Trim()) || x.Code.Contains(input.KeySearch.Trim())));

            switch (input.SortBy)
            {
                case "price-asc":
                    productsQuery = productsQuery.OrderBy(x => x.DiscountPrice);
                    break;

                case "price-desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.DiscountPrice);
                    break;

                case "new":
                    productsQuery = productsQuery.OrderBy(x => x.Index);
                    break;
            }

            var products = await productsQuery.Skip(input.SkipCount)
                 .Take(input.MaxResultCount)
                 .Include(x => x.Menu)
                 .Include(x => x.ProductAttributes).ThenInclude(p => p.Attribute)
                 .ToListAsync();

            if (products is not { Count: > 0 })
            {
                var productResult = _mapper.Map<List<ProductDto>>(products);
                return new PagedResultDto<ProductDto>
                {
                    Items = productResult,
                    TotalCount = productResult.Count
                };
            }

            var productsDto = _mapper.Map<List<ProductDto>>(products);
            this.RestructureAttribute(productsDto);

            foreach (var product in productsDto)
            {
                product.ImageUrl = !string.IsNullOrEmpty(product.Image)
                    ? S3Path.GetS3Url(_configuration, product.Image)
                    : "";
            }
            var productsResult = _mapper.Map<List<ProductDto>>(productsDto);

            return new PagedResultDto<ProductDto>
            {
                Items = productsResult,
                TotalCount = string.IsNullOrEmpty(input.KeySearch) && !input.CategoryId.HasValue && !input.Status.HasValue && !input.MinPrice.HasValue && !input.MaxPrice.HasValue
                    ? _productRepository.Count()
                    : await productsQuery.CountAsync()
            };
        }

        public async Task<PagedResultDto<ProductDto>> GetAllForAdmin(GetProductInput input)
        {
            if (input.MaxPrice.HasValue && !input.MinPrice.HasValue)
            {
                input.MinPrice = 0;
            }
            if (!input.MaxPrice.HasValue && input.MinPrice.HasValue)
            {
                input.MinPrice = double.MaxValue;
            }

            var productsQuery = _productRepository
                .AsQueryable()
                .OrderByDescending(x => x.UpdatedDate)
                .Where(x => !input.CategoryId.HasValue || x.MenuId == input.CategoryId)
                .Where(x => !input.Status.HasValue || x.Status == input.Status)

                .Where(x => !(input.MinPrice.HasValue && input.MaxPrice.HasValue)
                            || (x.DiscountPrice >= input.MinPrice.GetValueOrDefault() && x.DiscountPrice <= input.MaxPrice.GetValueOrDefault()))

                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Name.Contains(input.KeySearch.Trim()) || x.Code.Contains(input.KeySearch.Trim())));

            switch (input.SortBy)
            {
                case "price-asc":
                    productsQuery = productsQuery.OrderBy(x => x.DiscountPrice);
                    break;

                case "price-desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.DiscountPrice);
                    break;

                case "new":
                    productsQuery = productsQuery.OrderBy(x => x.Index);
                    break;
            }

            var products = await productsQuery.Skip(input.SkipCount)
                 .Take(input.MaxResultCount)
                 .Include(x => x.Menu)
                  .Include(x => x.ProductAttributes).ThenInclude(p => p.Attribute)
                 .ToListAsync();

            if (products is not { Count: > 0 })
            {
                var productResult = _mapper.Map<List<ProductDto>>(products);
                return new PagedResultDto<ProductDto>
                {
                    Items = productResult,
                    TotalCount = productResult.Count
                };
            }

            var productsDto = _mapper.Map<List<ProductDto>>(products);
            this.RestructureAttribute(productsDto);

            foreach (var product in productsDto)
            {
                product.ImageUrl = !string.IsNullOrEmpty(product.Image)
                    ? S3Path.GetS3Url(_configuration, product.Image)
                    : "";
            }
            var productsResult = _mapper.Map<List<ProductDto>>(productsDto);

            return new PagedResultDto<ProductDto>
            {
                Items = productsResult,
                TotalCount = string.IsNullOrEmpty(input.KeySearch) && !input.CategoryId.HasValue && !input.Status.HasValue && !input.MinPrice.HasValue && !input.MaxPrice.HasValue
                    ? _productRepository.Count()
                    : _productRepository
                        .Count(x => (!input.CategoryId.HasValue || x.MenuId == input.CategoryId)
                            && (string.IsNullOrEmpty(input.KeySearch) || x.Name.Contains(input.KeySearch.Trim()))
                            && (!input.Status.HasValue || x.Status == input.Status)
                            && (!(input.MinPrice.HasValue && input.MaxPrice.HasValue) || (x.DiscountPrice >= input.MinPrice.GetValueOrDefault()
                                && x.DiscountPrice <= input.MaxPrice.GetValueOrDefault())))
            };
        }

        public Task<ProductDto> GetByAlias(string alias)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductDto>> GetProductRelateds(Guid productId)
        {
            var productRelateds = await _productRelatedRepository.AsQueryable()
                .Where(x => x.ProductId == productId).Select(x => x.ProductRelatedId).ToListAsync();

            var products = await _productRepository.AsQueryable()
                .Where(x => productRelateds.Any(y => y == x.Id)).ToListAsync();

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            productDtos.ForEach(x =>
            {
                x.ImageUrl = !string.IsNullOrEmpty(x.Image)
                    ? S3Path.GetS3Url(_configuration, x.Image)
                    : "";

                x.ProductAttributes =
                    _mapper.Map<List<ProductAttributeDto>>(_productAttributeRepository.Query(attr => attr.ProductId == x.Id).ToList());

                x.ProductImages =
                    _mapper.Map<List<ProductImageDto>>(_productImageRepository.Query(img => img.ProductId == x.Id).ToList());

                if (x.ProductImages is { Count: > 0 })
                {
                    foreach (var productImage in x.ProductImages)
                    {
                        productImage.ImageUrl = !string.IsNullOrEmpty(productImage.Image)
                            ? S3Path.GetS3Url(_configuration, productImage.Image)
                            : "";
                    }
                }
            });

            return productDtos;
        }

        public async Task<ProductDto> GetById(Guid key)
        {
            var product = await _productRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (product == null)
                throw new Exception(CustomResponseMessage.ProductDoesNotExist);

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.ImageUrl = !string.IsNullOrEmpty(product.Image)
                ? S3Path.GetS3Url(_configuration, product.Image)
                : "";

            productDto.ProductAttributes =
                _mapper.Map<List<ProductAttributeDto>>(_productAttributeRepository.Query(x => x.ProductId == product.Id).ToList());

            productDto.ProductImages =
                _mapper.Map<List<ProductImageDto>>(_productImageRepository.Query(x => x.ProductId == product.Id).ToList());

            if (productDto.ProductImages is { Count: > 0 })
            {
                foreach (var productImage in productDto.ProductImages)
                {
                    productImage.ImageUrl = !string.IsNullOrEmpty(productImage.Image)
                        ? S3Path.GetS3Url(_configuration, productImage.Image)
                        : "";
                }
            }

            productDto.ProductRelateds =
                _mapper.Map<List<ProductRelatedDto>>(_productRelatedRepository.Query(x => x.ProductId == product.Id).ToList());

            return productDto;
        }

        public async Task<ProductDto> GetByIdForClient(Guid key)
        {
            var product = await _productRepository
                .AsQueryable()
                .Include(x => x.ProductAttributes).ThenInclude(x => x.Attribute)
                .FirstOrDefaultAsync(x => x.Id == key);
            if (product == null)
                throw new Exception(CustomResponseMessage.ProductDoesNotExist);

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.ImageUrl = !string.IsNullOrEmpty(product.Image)
                ? S3Path.GetS3Url(_configuration, product.Image)
                : "";

            productDto.ProductImages =
                _mapper.Map<List<ProductImageDto>>(_productImageRepository.Query(x => x.ProductId == product.Id).ToList());

            if (productDto.ProductImages is { Count: > 0 })
            {
                foreach (var productImage in productDto.ProductImages)
                {
                    productImage.ImageUrl = !string.IsNullOrEmpty(productImage.Image)
                        ? S3Path.GetS3Url(_configuration, productImage.Image)
                        : "";
                }
            }

            productDto.ProductImages.Add(new ProductImageDto
            {
                Id = Guid.NewGuid(),
                Image = productDto.Image,
                ImageUrl = productDto.ImageUrl,
                ProductId = productDto.Id
            });

            RestructureAttribute(new List<ProductDto> { productDto });
            return productDto;
        }

        public Task<List<ProductDto>> GetProductSelling(int number = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductDto> Insert(ProductDto entity, int userId)
        {
            var checkAlias = await _productRepository.AnyAsync(x => x.Alias == entity.Alias.Trim().ToLower());
            if (checkAlias)
            {
                throw new Exception("ALIAS_HAS_EXISTED");
            }

            var product = new Product()
            {
                Id = entity.Id,
                Price = entity.Price,
                Name = entity.Name,
                MenuId = entity.MenuId,
                Index = entity.Index,
                Image = entity.Image,
                DiscountPrice = entity.DiscountPrice,
                Selling = entity.Selling,
                Description = entity.Description,
                Alias = entity.Alias,
                Code = Guid.NewGuid().ToString().Truncate(8).ToUpper(),
                ShortDescription = entity.ShortDescription,
                Status = entity.Status,
                Quantity = entity.Quantity,
                RateAvg = entity.RateAvg,

                MetaTitle = entity.MetaTitle,
                MetaDescription = entity.MetaDescription,

                CreatedBy = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedBy = userId,
                UpdatedDate = DateTimeOffset.UtcNow,
            };

            if (entity.ProductImages.Count > 0)
            {
                var productImages =
                    (from item in entity.ProductImages
                     where !string.IsNullOrWhiteSpace(item.Image)
                     select new ProductImage()
                     {
                         Id = item.Id,
                         ProductId = product.Id,
                         Image = item.Image
                     })
                    .ToList();

                product.ProductImages = productImages;
            }
            if (entity.ProductAttributes is { Count: > 0 })
            {
                product.ProductAttributes = entity.ProductAttributes.Select(x => new ProductAttribute()
                {
                    AttributeId = x.AttributeId,
                    ProductId = x.ProductId,
                    Value = x.Value
                }).ToList();
            }
            if (entity.ProductRelateds.Count > 0)
            {
                product.ProductRelateds = entity.ProductRelateds.Select(x => new ProductRelated()
                {
                    ProductRelatedId = x.ProductRelatedId,
                    ProductId = x.ProductId
                }).ToList();
            }

            product.Alias = entity.Alias + "-" + product.Id;
            await _productRepository.AddAsync(product);
            return entity;
        }

        public void RestructureAttribute(List<ProductDto> products)
        {
            foreach (var product in products)
            {
                product.Attributes = product?.ProductAttributes?.Select(x => new AttributeDto()
                {
                    Name = x.Attribute.Name,
                    Id = x.Attribute.Id
                }).Distinct().ToList() ?? new List<AttributeDto>();

                product?.Attributes?.ForEach(x =>
                {
                    x.ProductAttributes = product?.ProductAttributes?
                        .Where(y => y.AttributeId == x.Id)
                        .Select(y => y.Value)
                        .FirstOrDefault()?.Split(',')
                        .Select(y => new ProductAttributeDto()
                        {
                            Value = y,
                        }).ToList() ?? new List<ProductAttributeDto>();
                });
                if (product != null) product.ProductAttributes = null!;
            }
        }

        public async Task Update(ProductDto entity, int userId)
        {
            var product = await _productRepository.FirstOrDefaultAsync(x => x.Id == entity.Id);
            if (product == null)
                throw new Exception("PRODUCT_DOES_NOT_EXIST");

            product.ProductAttributes = _productAttributeRepository.Query(x => x.ProductId == product.Id).ToList();
            product.ProductImages = _productImageRepository.Query(x => x.ProductId == product.Id).ToList();
            product.ProductRelateds = _productRelatedRepository.Query(x => x.ProductId == product.Id).ToList();

            product.MenuId = entity.MenuId;
            product.Name = entity.Name;

            if (product.Alias != entity.Alias)
            {
                var checkAlias = await _productRepository.AnyAsync(x => x.Id != product.Id && x.Alias == entity.Alias.Trim().ToLower());
                if (checkAlias)
                {
                    throw new Exception("ALIAS_HAS_EXISTED");
                }
                product.Alias = entity.Alias;
            }
            product.Image = entity.Image;
            product.Index = entity.Index;
            product.Status = entity.Status;
            product.Price = entity.Price;
            product.DiscountPrice = entity.DiscountPrice;
            product.Selling = entity.Selling;
            product.ShowPrice = entity.ShowPrice;
            product.ShortDescription = entity.ShortDescription;
            product.Description = entity.Description;
            product.RateAvg = entity.RateAvg;
            product.Quantity = entity.Quantity;

            product.MetaTitle = entity.MetaTitle;
            product.MetaDescription = entity.MetaDescription;

            product.UpdatedBy = userId;
            product.UpdatedDate = DateTimeOffset.Now;

            await HandleUpdateProductRefData(entity, product);
            await _productRepository.UpdateAsync(product);
        }

        private async Task HandleUpdateProductRefData(ProductDto inputData, Product currentData)
        {
            #region Attribute

            var deleteAttributesIds = currentData.ProductAttributes
                .Select(attr => attr.Id)
                .Except(inputData.ProductAttributes.Select(attr => attr.Id)).ToList();

            var addAttributesIds = inputData.ProductAttributes
                .Select(attr => attr.Id).Except(currentData.ProductAttributes.Select(attr => attr.Id)).ToList();

            if (deleteAttributesIds.Count > 0)
            {
                await _productAttributeRepository
                    .DeleteRangeAsync(currentData.ProductAttributes.Where(x => deleteAttributesIds.Exists(id => x.Id == id)));
            }
            if (addAttributesIds.Count > 0)
            {
                await _productAttributeRepository.AddRangeAsync(inputData.ProductAttributes?.Where(x => addAttributesIds.Exists(id => x.Id == id)).Select(x => new ProductAttribute()
                {
                    AttributeId = x.AttributeId,
                    ProductId = currentData.Id,
                    Value = x.Value
                })!);
            }

            #endregion Attribute

            #region ProductImages

            var deleteProductImageIds = currentData.ProductImages
                .Select(img => img.Id)
                .Except(inputData.ProductImages.Select(img => img.Id)).ToList();

            var addProductImageIds = inputData.ProductImages.Select(img => img.Id).Except(currentData.ProductImages.Select(img => img.Id)).ToList();

            if (deleteProductImageIds.Count > 0)
            {
                await _productImageRepository
                    .DeleteRangeAsync(currentData.ProductImages.Where(x => deleteProductImageIds.Exists(id => x.Id == id)));
            }

            if (addProductImageIds.Count > 0)
            {
                await _productImageRepository
                    .AddRangeAsync(inputData.ProductImages.Where(x => addProductImageIds.Exists(id => x.Id == id) && !string.IsNullOrWhiteSpace(x.Image)).Select(x => new ProductImage()
                    {
                        ProductId = currentData.Id,
                        Image = x.Image
                    }));
            }

            #endregion ProductImages

            #region ProductRelateds

            var deleteProductRelatedIds = currentData.ProductRelateds
                .Select(img => img.Id)
                .Except(inputData.Attributes.Select(img => img.Id)).ToList();

            var addProductRelatedIds = inputData.ProductRelateds.Select(img => img.Id).Except(currentData.ProductRelateds.Select(img => img.Id)).ToList();

            if (deleteProductRelatedIds.Count > 0)
            {
                await _productRelatedRepository
                    .DeleteRangeAsync(currentData.ProductRelateds.Where(x => deleteProductRelatedIds.Exists(id => x.Id == id)));
            }

            if (addProductRelatedIds.Count > 0)
            {
                await _productRelatedRepository
                    .AddRangeAsync(inputData.ProductRelateds.Where(x => addProductRelatedIds.Exists(id => x.Id == id))
                        .Select(x => new ProductRelated()
                        {
                            ProductId = currentData.Id,
                            ProductRelatedId = x.ProductRelatedId
                        }));
            }

            #endregion ProductRelateds
        }
    }
}