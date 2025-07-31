using Application.Interfaces.Repositories.EFCore;

namespace Application.Interfaces.Repositories.EfCore
{
    public interface IAttributeRepository : IGenericRepository<Domain.Entities.Attribute>
    {
    }
}