using Application.Dtos;
using Application.Dtos.Menu;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IMenuService
    {
        /// <summary>
        /// Xóa danh mục menu
        /// </summary>
        /// <param name="menuId"></param>
        Task DeleteById(int menuId);

        /// <summary>
        /// Get tất cả menu
        /// </summary>
        /// <returns></returns>
        Task<PagedResultDto<MenuDto>> GetAll(GetMenuInput input);

        /// <summary>
        /// Get danh sách menu chính theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        Task<List<MenuDto>> GetMainMenu(string? keySearch);

        /// <summary>
        /// Get danh sách menu phụ theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        Task<List<MenuDto>> GetSubMenu(string? keySearch);

        /// <summary>
        /// Get tất cả danh sách menu chính được active
        /// </summary>
        /// <returns></returns>
        Task<List<MenuDto>> GetMainMenuActive();

        /// <summary>
        /// Get tất cả danh sách menu phụ được active
        /// </summary>
        /// <returns></returns>
        Task<List<MenuDto>> GetSubMenuActive();

        /// <summary>
        /// Get tất cả danh sách menu cha chính
        /// </summary>
        /// <returns></returns>
        Task<List<MenuDto>> GetParentMainMenu();

        /// <summary>
        /// Get tất cả danh sách menu cha phụ
        /// </summary>
        /// <returns></returns>
        Task<List<MenuDto>> GetParentSubMenu();

        /// <summary>
        /// Get danh sách menu theo loại
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        Task<List<MenuDto>> GetByType(List<string> types);

        Task<List<MenuDto>> GetByTypeForAdmin(List<string> types);

        /// <summary>
        /// Get tất cả menu hiển thị trang chủ
        /// </summary>
        /// <returns></returns>
        Task<List<MenuDto>> GetAllShowHomePage();

        Task<MenuDto> GetById(int key);

        /// <summary>
        /// Get menu theo alias
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<MenuDto> GetByAlias(string alias);

        Task<MenuDto> Insert(MenuDto entity);

        Task Update(int key, MenuDto entity);
    }
}