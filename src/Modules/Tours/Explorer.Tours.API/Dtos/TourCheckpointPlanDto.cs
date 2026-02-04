using System;

namespace Explorer.Tours.API.Dtos
{
    public class TourCheckpointPlanDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PlannerItemId { get; set; }
        public long KeyPointId { get; set; }
        public DateTime PlannedAt { get; set; }
    }
}
