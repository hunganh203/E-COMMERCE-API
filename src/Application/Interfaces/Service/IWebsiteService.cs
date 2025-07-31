using Application.Dtos;

namespace Application.Interfaces.Service
{
    public interface IWebsiteService
    {
        Task DeleteByIdAsync(int key);

        Task<List<WebsiteDto>> GetAllAsync();

        Task<WebsiteDto> GetByIdAsync(int key);

        Task<WebsiteDto> InsertAsync(WebsiteDto entity);

        Task UpdateAsync(WebsiteDto entity);
    }
}