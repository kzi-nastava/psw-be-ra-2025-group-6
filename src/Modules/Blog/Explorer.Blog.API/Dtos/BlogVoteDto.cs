namespace Explorer.Blog.API.Dtos
{
    public class BlogVoteDto
    {
        public long Id { get; set; }
        public long BlogPostId { get; set; }
        public long UserId { get; set; }
        public DateTime VotedAt { get; set; }
        public VoteTypeDto Type { get; set; }
    }
}
