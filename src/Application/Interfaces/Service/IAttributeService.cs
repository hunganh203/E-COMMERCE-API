using Application.Dtos;

namespace Application.Interfaces.Service
{
    public interface IAttributeService
    {
        /// <summary>
        /// Xóa thuộc tính
        /// </summary>
        /// <param name="key"></param>
        Task DeleteById(int key);

        /// <summary>
        /// Get thuộc tính theo từ khóa
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        Task<List<AttributeDto>> Get(string? keySearch);

        /// <summary>
        /// Get tất cả thuộc tính
        /// </summary>
        /// <returns></returns>
        Task<List<AttributeDto>> GetAll();

        /// <summary>
        /// Get thuộc tính theo id
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<AttributeDto> GetById(int key);

        /// <summary>
        /// Thêm mới thuộc tính
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<AttributeDto> Insert(AttributeDto entity);

        /// <summary>
        /// Cập nhật thuộc tính
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        Task Update(int key, AttributeDto entity);
    }
}