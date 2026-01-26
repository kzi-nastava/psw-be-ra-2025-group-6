using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository : ICrudRepository<Notification>
    {
        List<Notification> GetUnreadByRecipient(long recipientId);
        
        // ? NEW: Leaderboard-specific queries
        List<Notification> GetByRecipientAndType(long recipientId, NotificationType type);
        List<Notification> GetUnreadByRecipientAndType(long recipientId, NotificationType type);
    }
}
