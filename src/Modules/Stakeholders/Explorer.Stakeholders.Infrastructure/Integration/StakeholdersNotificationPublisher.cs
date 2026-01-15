using Explorer.BuildingBlocks.Core.Integration;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Infrastructure.Integration;

public class StakeholdersNotificationPublisher : INotificationPublisher
{
    private readonly INotificationService _notificationService;

    public StakeholdersNotificationPublisher(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void PublishNotification(long recipientId, long senderId, string content, long referenceId)
    {
        var dto = new NotificationDto
        {
            RecipientId = recipientId,
            SenderId = senderId,
            Content = content,
            Status = "Unread",
            Timestamp = DateTime.UtcNow,
            ReferenceId = referenceId
        };

        _notificationService.Create(dto);
    }
}
