using Application.Dtos;

namespace Ecommerce.Service
{
    public interface IEmailRegistrationService
    {
        /// <summary>
        /// Get danh sách email đăng ký nhận tin
        /// </summary>
        /// <param name="keySearch"></param>
        /// <returns></returns>
        Task<List<EmailRegistrationDto>> Get(string keySearch);

        /// <summary>
        /// Thêm mới email nhận tin
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<EmailRegistrationDto> Insert(EmailRegistrationDto entity);
    }
}