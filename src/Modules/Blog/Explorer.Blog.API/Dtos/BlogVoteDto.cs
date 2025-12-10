namespace Explorer.Blog.API.Dtos
{
    public class BlogVoteDto
    {
        public long UserId { get; set; }
        public DateTime VotedAt { get; set; }
        public VoteTypeDto Type { get; set; }
    }
}
