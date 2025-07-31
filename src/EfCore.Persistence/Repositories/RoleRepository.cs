using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}