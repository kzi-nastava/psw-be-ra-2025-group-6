using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public class TourProblemMessage : Entity
    {
        public long TourProblemId { get; private set; }
        public long SenderId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Content { get; private set; }

        private TourProblemMessage() { /* Required for EF Core */ }

        public TourProblemMessage(long tourProblemId, long senderId, string content)
        {
            TourProblemId = tourProblemId;
            SenderId = senderId;
            Timestamp = DateTime.UtcNow;
            Content = content;
            Validate();
        }

        private void Validate()
        {
            if (TourProblemId <= 0) throw new ArgumentException("Invalid TourProblemId");
            if (SenderId <= 0) throw new ArgumentException("Invalid SenderId");
            if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Content cannot be empty");
        }
    }
}
