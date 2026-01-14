using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Notification : Entity
    {
        public long RecipientId { get; private set; }
        public long SenderId { get; private set; }
        public string Content { get; private set; }
        public NotificationStatus Status { get; private set; }
        public DateTime Timestamp { get; private set; }
        public long ReferenceId { get; private set; } // e.g., TourProblemId or TourProblemMessageId

        private Notification() { /* Required for EF Core */ }

        public Notification(long recipientId, long senderId, string content, long referenceId)
        {
            RecipientId = recipientId;
            SenderId = senderId;
            Content = content;
            Status = NotificationStatus.Unread;
            Timestamp = DateTime.UtcNow;
            ReferenceId = referenceId;
            Validate();
        }

        public void MarkAsRead()
        {
            Status = NotificationStatus.Read;
        }

        private void Validate()
        {
            if (SenderId <= 0) throw new ArgumentException("Invalid SenderId");
            if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Content cannot be empty");
            if (ReferenceId <= 0) throw new ArgumentException("Invalid ReferenceId");
        }
    }

    public enum NotificationStatus
    {
        Unread,
        Read
    }
}
