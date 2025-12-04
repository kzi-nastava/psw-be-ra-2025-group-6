using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface INotificationService
    {
        NotificationDto Create(NotificationDto notification);
        List<NotificationDto> GetUnreadByRecipient(long recipientId);
        NotificationDto MarkAsRead(long notificationId);
    }
}
