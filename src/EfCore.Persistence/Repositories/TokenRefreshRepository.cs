using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class TokenRefreshRepository : GenericRepository<TokenRefresh>, ITokenRefreshRepository
    {
        public TokenRefreshRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}