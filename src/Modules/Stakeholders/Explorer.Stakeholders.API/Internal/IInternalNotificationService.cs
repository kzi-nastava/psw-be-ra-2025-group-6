namespace Explorer.Stakeholders.API.Internal;

/// <summary>
/// Internal API for notification creation - used by other modules (e.g., Encounters, Tours, Blog)
/// </summary>
public interface IInternalNotificationService
{
    /// <summary>
    /// Create a notification for a single recipient
    /// </summary>
    void CreateNotification(long recipientId, long senderId, string content, long? referenceId, string type);
    
    /// <summary>
    /// Create notifications for multiple recipients
    /// </summary>
    void CreateNotificationsForMultipleRecipients(List<long> recipientIds, long senderId, string content, long? referenceId, string type);
}
