using Application.Interfaces.Repositories.EFCore;
using Domain.Entities;
using EfCore.Persistence.Contexts;

namespace EFCore.Persistence.Repositories
{
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}