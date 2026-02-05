namespace Shared.Notifications;

public record NotificationRequestedEvent(
    long RecipientId,
    long SenderId,
    string Content,
    long ReferenceId
) : IDomainEvent;

