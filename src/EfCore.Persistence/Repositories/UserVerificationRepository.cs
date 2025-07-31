using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class UserVerificationRepository : GenericRepository<UserVerification>, IUserVerificationRepository
    {
        public UserVerificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}