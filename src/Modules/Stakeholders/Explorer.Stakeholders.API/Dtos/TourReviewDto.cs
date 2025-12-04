namespace Explorer.Stakeholders.API.Dtos
{
    public class TourReviewDto
    {
        public long Id { get; set; }
        public long UserId { get; init; }
        public long TourId { get; init; }
        public int Rating { get; set; }
        public int CompletedPercent { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
