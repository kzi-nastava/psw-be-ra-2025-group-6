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
    }
}
