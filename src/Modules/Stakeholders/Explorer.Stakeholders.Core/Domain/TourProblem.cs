using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public class TourProblem : AggregateRoot
    {
        public long TourId { get; private set; }
        public long TouristId { get; private set; }
        public ProblemCategory Category { get; private set; }
        public ProblemPriority Priority { get; private set; }
        public string Description { get; private set; }
        public DateTime ReportedAt { get; private set; }
        public ProblemStatus Status { get; private set; }
        public DateTime? DeadlineAt { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public ProblemResolutionFeedback ResolutionFeedback { get; private set; }
        public string? ResolutionComment { get; private set; }
        public DateTime? ResolutionAt { get; private set; }

        
        private TourProblem() { Description = string.Empty; }

        public TourProblem(long tourId, long touristId, ProblemCategory category,
                          ProblemPriority priority, string description)
        {
            TourId = tourId;
            TouristId = touristId;
            Category = category;
            Priority = priority;
            Description = description;
            ReportedAt = DateTime.UtcNow;
            Status = ProblemStatus.Open;
            ResolutionFeedback = ProblemResolutionFeedback.Pending;

            Validate();
        }

        public void Update(ProblemCategory category, ProblemPriority priority, string description)
        {
            Category = category;
            Priority = priority;
            Description = description;

            Validate();
        }

        public void SetDeadline(DateTime deadlineUtc)
        {
            if (DeadlineAt.HasValue) throw new InvalidOperationException("Deadline already set for this problem.");
            if (Status != ProblemStatus.Open) throw new InvalidOperationException("Cannot change deadline on a closed problem.");
            if (deadlineUtc <= DateTime.UtcNow) throw new ArgumentException("Deadline must be in the future.");
            DeadlineAt = deadlineUtc;
        }

        public void MarkClosed(DateTime closedAtUtc)
        {
            Status = ProblemStatus.Closed;
            ResolvedAt = closedAtUtc;
        }

        public void FinalizeStatus(ProblemStatus newStatus, DateTime finalizedAtUtc)
        {
            if (newStatus == ProblemStatus.Open)
                throw new ArgumentException("Cannot finalize to Open state.");
            if (Status != ProblemStatus.Open)
                throw new InvalidOperationException("Problem already finalized.");
            if (ResolutionFeedback == ProblemResolutionFeedback.Pending)
                throw new InvalidOperationException("Cannot finalize without resolution feedback.");

            Status = newStatus;
            ResolvedAt = finalizedAtUtc;
        }

        public void SetResolutionFeedback(ProblemResolutionFeedback feedback, string? comment)
        {
            if (feedback == ProblemResolutionFeedback.Pending)
                throw new ArgumentException("Resolution feedback must be Resolved or NotResolved.");

            if (feedback == ProblemResolutionFeedback.NotResolvedByTourist && string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Resolution comment is required when marking as not resolved.");

            ResolutionFeedback = feedback;
            ResolutionComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            ResolutionAt = DateTime.UtcNow;
        }

        public bool IsOverdue(DateTime utcNow)
        {
            if (Status != ProblemStatus.Open) return false;
            if (DeadlineAt.HasValue && utcNow > DeadlineAt.Value) return true;
            return ReportedAt <= utcNow.AddDays(-5);
        }

        private void Validate()
        {
            if (TourId <= 0) throw new ArgumentException("Invalid TourId");
            if (TouristId <= 0) throw new ArgumentException("Invalid TouristId");
            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Description cannot be empty");
            if (Description.Length > 1000)
                throw new ArgumentException("Description too long");
        }
    }

    public enum ProblemCategory
    {
        Technical = 0,
        Route = 1,
        Safety = 2,
        Other = 3
    }

    public enum ProblemPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public enum ProblemStatus
    {
        Open = 0,
        Closed = 1,
        Penalized = 2
    }

    public enum ProblemResolutionFeedback
    {
        Pending = 0,
        ResolvedByTourist = 1,
        NotResolvedByTourist = 2
    }
}
