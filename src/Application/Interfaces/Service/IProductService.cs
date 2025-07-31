using Application.Dtos.Product;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IProductService
    {
        Task DeleteById(Guid key);

        Task<List<ProductDto>> GetProductsForSelect(GetProductsForSelect input);

        Task<PagedResultDto<ProductDto>> GetAll(GetProductInput input);

        Task<PagedResultDto<ProductDto>> GetAllForAdmin(GetProductInput input);

        Task<List<ProductDto>> GetProductSelling(int number = 10);

        Task<ProductDto> GetById(Guid key);

        Task<ProductDto> GetByIdForClient(Guid key);

        Task<ProductDto> GetByAlias(string alias);

        Task<List<ProductDto>> GetProductRelateds(Guid productId);

        Task<ProductDto> Insert(ProductDto entity, int userId);

        Task Update(ProductDto entity, int userId);

        void RestructureAttribute(List<ProductDto> products);
    }
}