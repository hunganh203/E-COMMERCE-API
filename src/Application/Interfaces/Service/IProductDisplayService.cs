using Application.Dtos.Product;
using Application.Dtos.ProductDisplay;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IProductDisplayService
    {
        Task<PagedResultDto<ProductDisplayDto>> GetAll(GetProductDisplaysInput input);

        Task<PagedResultDto<ProductDto>> GetForClient(GetProductDisplaysInput input);

        Task<ProductDisplayDto> GetById(Guid id);

        Task<List<ProductDisplayDto>> Add(ProductDisplayAddDto input);

        Task<ProductDisplayDto> Update(ProductDisplayDto input);

        Task<bool> Delete(Guid id);

        Task<bool> Delete(List<Guid> ids);
    }
}