using Application.Dtos.UserNotification;
using Application.Interfaces.Repositories.EFCore;
using Application.Interfaces.Service;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Shared.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IUserNotificationRepository _userNotificationRepository;

        public UserNotificationService(IUserNotificationRepository userNotificationRepository)
        {
            _userNotificationRepository = userNotificationRepository;
        }

        //public async Task<PagedResultDto<UserNotificationDto>> GetNotificationsByUser(
        //   GetNotificationInput input,
        //   int userId)
        //{
        //    var isOnlyShowUnread = false;
        //    var notifications = await _userNotificationRepository
        //        .AsQueryable().Where(x => x.UserId == userId && x);

        //    return notifications;
        //}

        public async Task<int> CountUnreadNotificationsByUser(int userId, int organizationId)
        {
            var notifications = _userNotificationRepository
                .Query(x => x.UserId == userId &&
                            !x.IsDeleted && !x.Status)
                .OrderByDescending(x => x.CreatedDate);

            return await notifications.CountAsync();
        }

        public async Task UpdateNotification(int userId, UpdateNotificationDto input, int organizationId)
        {
            var userNotifications = new List<UserNotification>();
            if (input.SelectAll == true)
            {
                var notifications = await _userNotificationRepository
                    .Query(x => x.UserId == userId && !x.IsDeleted)
                    .OrderByDescending(x => x.CreatedDate).ToListAsync();

                userNotifications.AddRange(notifications);
            }
            else
            {
                var notifications = await _userNotificationRepository
                    .FirstOrDefaultAsync(x => x.UserId == userId &&
                                              x.Id == input.NotificationId);
                if (notifications == null)
                {
                    throw new Exception("Notification does not exist");
                }
                userNotifications.Add(notifications);
            }

            userNotifications.ForEach(notification =>
            {
                notification.Status = input.Status;
                notification.ModifiedDate = DateTime.UtcNow;
            });

            _userNotificationRepository.UpdateRange(userNotifications);

            // this.HandleEmitActivityNotify(userId, organizationId);
        }

        public async Task DeleteNotification(List<int> notificationIds, int userId, int organizationId)
        {
            if (!notificationIds.Any()) return;

            var notifications = await _userNotificationRepository
                .Query(x =>
                    notificationIds.Contains(x.Id) &&
                    x.UserId == userId)
                .ToListAsync();

            notifications.ForEach(item =>
            {
                item.IsDeleted = true;
                item.ModifiedDate = DateTime.UtcNow;
            });

            _userNotificationRepository.UpdateRange(notifications);
            //  this.HandleEmitActivityNotify(userId, organizationId);
        }

        //private async Task<bool> IsUserSetOnlyShowUnread(int userId, int organizationId)
        //{
        //    return false;
        //}
    }
}