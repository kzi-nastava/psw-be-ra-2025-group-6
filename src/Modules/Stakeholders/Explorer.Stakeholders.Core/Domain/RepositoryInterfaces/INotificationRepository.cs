using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository : ICrudRepository<Notification>
    {
        List<Notification> GetUnreadByRecipient(long recipientId);
        List<Notification> GetByRecipient(long recipientId, int? limit = null);
        bool ExistsForRecipientAndReference(long recipientId, long referenceId, string title);
    }
}
