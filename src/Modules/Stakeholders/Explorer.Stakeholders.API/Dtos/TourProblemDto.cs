using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class TourProblemDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public long TouristId { get; set; }
        public int Category { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
        public DateTime ReportedAt { get; set; }
    }
}