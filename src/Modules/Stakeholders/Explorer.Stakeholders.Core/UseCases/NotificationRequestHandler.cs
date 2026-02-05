using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Shared;
using Shared.Notifications;

namespace Explorer.Stakeholders.Core.UseCases;
public class NotificationRequestHandler : IDomainEventHandler<NotificationRequestedEvent>
{
    private readonly INotificationService _notificationService;

    public NotificationRequestHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task Handle(NotificationRequestedEvent domainEvent)
    {
        _notificationService.Create(new NotificationDto
        {
            RecipientId = domainEvent.RecipientId,
            SenderId = domainEvent.SenderId,
            Content = domainEvent.Content,
            ReferenceId = domainEvent.ReferenceId
        });

        return Task.CompletedTask;
    }
}

