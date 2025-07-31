using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class EmailTemplateRepository : GenericRepository<EmailTemplate>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}