using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class TourProblemMessageDto
    {
        public long Id { get; set; }
        public long TourProblemId { get; set; }
        public long SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
    }
}
