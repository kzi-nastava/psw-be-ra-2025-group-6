using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Tours.Core.Domain
{
    public class TourProblem : Entity
    {
        public long TourId { get; private set; }
        public long TouristId { get; private set; }
        public ProblemCategory Category { get; private set; }
        public ProblemPriority Priority { get; private set; }
        public string Description { get; private set; }
        public DateTime ReportedAt { get; private set; }

        
        private TourProblem() { }

        public TourProblem(long tourId, long touristId, ProblemCategory category,
                          ProblemPriority priority, string description)
        {
            TourId = tourId;
            TouristId = touristId;
            Category = category;
            Priority = priority;
            Description = description;
            ReportedAt = DateTime.UtcNow;

            Validate();
        }

        public void Update(ProblemCategory category, ProblemPriority priority, string description)
        {
            Category = category;
            Priority = priority;
            Description = description;

            Validate();
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
}