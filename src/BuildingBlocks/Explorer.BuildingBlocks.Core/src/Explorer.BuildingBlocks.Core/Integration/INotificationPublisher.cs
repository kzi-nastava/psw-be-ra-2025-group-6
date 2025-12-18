namespace Explorer.BuildingBlocks.Core.Integration
{
    public interface INotificationPublisher
    {
        void PublishNotification(long recipientId, long senderId, string content, long referenceId);
    }
}
