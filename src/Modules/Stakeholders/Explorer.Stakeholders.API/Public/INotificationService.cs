using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface INotificationService
    {
        NotificationDto Create(NotificationDto notification);
        List<NotificationDto> GetUnreadByRecipient(long recipientId);
        List<NotificationDto> GetByRecipient(long recipientId, int? limit = null);
        NotificationDto MarkAsRead(long notificationId);
    }
}
