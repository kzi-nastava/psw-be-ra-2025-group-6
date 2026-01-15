namespace Explorer.Payments.API.Internal;

public interface INotificationDataProvider
{
    void CreateNotification(long recipientId, long senderId, string message, long referenceId);
}
