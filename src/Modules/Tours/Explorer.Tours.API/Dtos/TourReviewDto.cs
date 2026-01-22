namespace Explorer.Tours.API.Dtos
{
    public class TourReviewDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = "Anonymous"; 

        public long TourId { get; set; }
        public int Rating { get; set; }
        public int CompletedPercent { get; set; }
        public string? Comment { get; set; }
        public string PictureUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
