using Application.Interfaces.Repositories.EfCore;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class AttributeRepository : GenericRepository<Domain.Entities.Attribute>, IAttributeRepository
    {
        public AttributeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}