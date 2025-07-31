using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class EmailRegistrationRepository : GenericRepository<EmailRegistration>, IEmailRegistrationRepository
    {
        public EmailRegistrationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}