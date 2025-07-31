using Application.Interfaces.Repositories.EFCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;

namespace EFCore.Persistence.Repositories
{
    public class UserDeviceTokenRepository : GenericRepository<UserDeviceToken>, IUserDeviceTokenRepository
    {
        public UserDeviceTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}