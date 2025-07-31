using Application.Dtos;

namespace Ecommerce.Service
{
    public interface IGalleryService
    {
        Task DeleteById(int key, string? userSession = null);

        Task<List<GalleryDto>> GetAll();

        Task<GalleryDto> GetById(int key);

        Task<GalleryDto> Insert(GalleryDto entity);

        Task Update(int key, GalleryDto entity);
    }
}