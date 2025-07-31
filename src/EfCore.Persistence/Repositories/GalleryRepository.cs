using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class GalleryRepository : GenericRepository<Gallery>, IGalleryRepository
    {
        public GalleryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}