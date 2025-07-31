using Application.Interfaces.Repositories.EFCore;
using Domain.Entities;

namespace Application.Interfaces.Repositories.EfCore
{
    public interface IEmailRegistrationRepository : IGenericRepository<EmailRegistration>
    {
    }
}