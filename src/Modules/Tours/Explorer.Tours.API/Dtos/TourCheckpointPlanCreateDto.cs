using System;

namespace Explorer.Tours.API.Dtos
{
    public class TourCheckpointPlanCreateDto
    {
        public long PlannerItemId { get; set; }
        public long KeyPointId { get; set; }
        public DateTime PlannedAt { get; set; }
    }
}
