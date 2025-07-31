using Application.Interfaces.Repositories.EfCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;
using EFCore.Persistence.Repositories;

namespace EfCore.Persistence.Repositories
{
    public class WebsiteRepository : GenericRepository<Website>, IWebsiteRepository
    {
        public WebsiteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}