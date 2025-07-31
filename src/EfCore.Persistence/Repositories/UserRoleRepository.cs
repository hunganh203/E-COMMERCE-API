using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}