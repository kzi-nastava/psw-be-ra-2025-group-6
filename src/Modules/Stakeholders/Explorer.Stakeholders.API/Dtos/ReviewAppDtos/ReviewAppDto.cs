namespace Explorer.Stakeholders.API.Dtos.ReviewAppDtos
{
    public class ReviewAppDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
