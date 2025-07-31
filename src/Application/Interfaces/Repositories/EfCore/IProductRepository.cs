using Application.Interfaces.Repositories.EFCore;
using Domain.Entities;

namespace Application.Interfaces.Repositories.EfCore
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task DeleteById(Guid key);
    }
}