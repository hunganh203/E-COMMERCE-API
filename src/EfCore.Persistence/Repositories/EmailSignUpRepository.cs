using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class EmailSignUpRepository : GenericRepository<EmailSignUp>, IEmailSignUpRepository
    {
        public EmailSignUpRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}