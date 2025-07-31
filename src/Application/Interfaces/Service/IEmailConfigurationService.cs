using Application.Dtos;

namespace Ecommerce.Service
{
    public interface IEmailConfigurationService
    {
        /// <summary>
        /// Get cấu hình tài khoản gửi mail
        /// </summary>
        /// <returns></returns>
        Task<EmailConfigurationDto> Get();

        /// <summary>
        /// Cập nhật tài khoản gửi mail
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        Task Update(int key, EmailConfigurationDto entity);
    }
}