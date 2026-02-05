using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface INotificationService
    {
        NotificationDto Create(NotificationDto notification);
        List<NotificationDto> GetUnreadByRecipient(long recipientId);
        List<NotificationDto> GetUnreadByRecipientAndType(long recipientId, string type);
        List<NotificationDto> GetByRecipientAndType(long recipientId, string type);
        NotificationDto MarkAsRead(long notificationId);
    }
}
