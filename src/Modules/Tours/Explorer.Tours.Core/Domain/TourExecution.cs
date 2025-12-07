using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public enum TourExecutionStatus { active, completed, abandoned }

    public class TourExecution : Entity
    {
        public long TourId { get; private set; }
        public long TouristId { get; private set; }
        public TourExecutionStatus Status { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public TrackPoint InitialPosition { get; private set; }
        public List<long> ExecutionKeyPoints { get; private set; } = new();
        public DateTime LastActivity { get; private set; }

        private TourExecution() { }

        public TourExecution(long tourId, long touristId, TrackPoint initialPosition)
        {
            if (tourId <= 0) throw new ArgumentException("Invalid TourId");
            if (touristId <= 0) throw new ArgumentException("Invalid TouristId");
            TourId = tourId;
            TouristId = touristId;
            InitialPosition = initialPosition ?? throw new ArgumentNullException(nameof(initialPosition));
            Status = TourExecutionStatus.active;
            StartTime = DateTime.UtcNow;
            LastActivity = StartTime;
        }

        public void Complete()
        {
            Status = TourExecutionStatus.completed;
            EndTime = DateTime.UtcNow;
            LastActivity = EndTime.Value;
        }

        public void Abandon()
        {
            Status = TourExecutionStatus.abandoned;
            EndTime = DateTime.UtcNow;
            LastActivity = EndTime.Value;
        }

        public void UpdateLastActivity()
        {
            LastActivity = DateTime.UtcNow;
        }

        public void AddExecutionKeyPoint(long keyPointId)
        {
            ExecutionKeyPoints.Add(keyPointId);
            UpdateLastActivity();
        }
    }
}
