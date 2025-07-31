using Application.Dtos;

namespace Application.Interfaces.Service
{
    public interface IEmailTemplateService
    {
        Task<List<EmailTemplateDto>> GetAll();

        Task<EmailTemplateDto> GetById(int key);

        Task<EmailTemplateDto> Insert(EmailTemplateDto entity);

        Task Update(int key, EmailTemplateDto entity);
    }
}