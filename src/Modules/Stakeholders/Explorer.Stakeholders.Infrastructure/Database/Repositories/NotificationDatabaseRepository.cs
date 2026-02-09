using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class NotificationDatabaseRepository : CrudDatabaseRepository<Notification, StakeholdersContext>, INotificationRepository
    {
        private readonly StakeholdersContext _dbContext;
        public NotificationDatabaseRepository(StakeholdersContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Notification> GetUnreadByRecipient(long recipientId)
        {
            return _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId && n.Status == NotificationStatus.Unread)
                .OrderByDescending(n => n.Timestamp)
                .ToList();
        }

        public List<Notification> GetByRecipient(long recipientId)
        {
            return _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId)
                .OrderByDescending(n => n.Timestamp)
                .ToList();
        }

        // ? NEW: Get notifications by recipient and type
        public List<Notification> GetByRecipientAndType(long recipientId, NotificationType type)
        {
            return _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId && n.Type == type)
                .OrderByDescending(n => n.Timestamp)
                .ToList();
        }

        // ? NEW: Get unread notifications by recipient and type
        public List<Notification> GetUnreadByRecipientAndType(long recipientId, NotificationType type)
        {
            return _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId && n.Status == NotificationStatus.Unread && n.Type == type)
                .OrderByDescending(n => n.Timestamp)
                .ToList();
        }

        public List<Notification> GetByRecipient(long recipientId, int? limit = null)
        {
            var query = _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId);

            if (limit.HasValue)
            {
                return query
                    .OrderByDescending(n => n.Timestamp)
                    .Take(limit.Value)
                    .ToList();
            }

            return query
                .OrderByDescending(n => n.Timestamp)
                .ToList();
        }

        public bool ExistsForRecipientAndReference(long recipientId, long referenceId, string title)
        {
            return _dbContext.Notifications.Any(n =>
                n.RecipientId == recipientId &&
                n.ReferenceId == referenceId &&
                n.Title == title);
        }
    }
}
