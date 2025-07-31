using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class ProductAttributeRepository : GenericRepository<ProductAttribute>, IProductAttributeRepository
    {
        public ProductAttributeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}