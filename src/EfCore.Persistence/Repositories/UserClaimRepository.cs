using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class UserClaimRepository : GenericRepository<UserClaim>, IUserClaimRepository
    {
        public UserClaimRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}