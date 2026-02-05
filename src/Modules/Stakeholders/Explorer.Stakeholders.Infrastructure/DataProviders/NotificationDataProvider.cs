using Explorer.Payments.API.Internal;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Infrastructure.DataProviders;

public class NotificationDataProvider : INotificationDataProvider
{
    private readonly INotificationService _notificationService;

    public NotificationDataProvider(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void CreateNotification(long recipientId, long senderId, string message, long referenceId)
    {
        var notification = new NotificationDto
        {
            RecipientId = recipientId,
            SenderId = senderId,
            Content = message,
            Timestamp = DateTime.UtcNow,
            Status = "UNREAD",
            ReferenceId = referenceId
        };
        _notificationService.Create(notification);
    }
}
