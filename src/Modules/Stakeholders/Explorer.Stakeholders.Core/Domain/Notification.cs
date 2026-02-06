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
        public NotificationType Type { get; private set; }
        public DateTime Timestamp { get; private set; }
        public long ReferenceId { get; private set; } // e.g., TourProblemId or TourProblemMessageId

        private Notification() { /* Required for EF Core */ }

        public Notification(long recipientId, long senderId, string content, long referenceId, NotificationType type = NotificationType.General)
        {
            RecipientId = recipientId;
            SenderId = senderId;
            Content = content;
            Status = NotificationStatus.Unread;
            Type = type;
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
            // Tests use negative IDs for seeded users; only zero is considered invalid
            if (RecipientId == 0) throw new ArgumentException("Invalid RecipientId");
            if (SenderId == 0) throw new ArgumentException("Invalid SenderId");
            if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Content cannot be empty");
            if (ReferenceId == 0) throw new ArgumentException("Invalid ReferenceId");
        }
    }

    public enum NotificationStatus
    {
        Unread,
        Read
    }

    public enum NotificationType
    {
        // General notifications
        General = 0,
        
        // Tour Problem notifications (existing functionality)
        TourProblemDeadlineSet = 10,
        TourProblemMessage = 11,
        TourProblemResolved = 12,
        TourProblemNotResolved = 13,
        TourProblemClosed = 14,
        TourProblemPenalized = 15,
        TourSuspended = 16,
        
        // Leaderboard notifications (new functionality)
        RankIncreased = 100,
        RankDecreased = 101,
        EnteredTop10 = 102,
        EnteredTop3 = 103,
        BecameFirst = 104,
        MilestoneXP = 105,
        MilestoneChallenges = 106,
        MilestoneTours = 107,
        ClubRankChanged = 108,
        ClubMemberContribution = 109,
        NearRankingAlert = 110,
        WeeklySummary = 111
    }
}
