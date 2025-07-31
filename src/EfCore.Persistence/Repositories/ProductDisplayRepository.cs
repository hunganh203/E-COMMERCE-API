using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class ProductDisplayRepository : GenericRepository<ProductDisplay>, IProductDisplayRepository
    {
        public ProductDisplayRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}