using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogVote : Entity
    {
        public long BlogPostId { get; private set; }
        public long UserId { get; private set; }
        public DateTime VotedAt { get; private set; }
        public VoteType Type { get; private set; }

        private BlogVote() { }

        public BlogVote(long blogPostId, long userId, VoteType type)
        {
            if (blogPostId == 0) throw new ArgumentException("Invalid BlogPostId.");
            if (userId == 0) throw new ArgumentException("Invalid UserId.");
            BlogPostId = blogPostId;
            UserId = userId;
            Type = type;
            VotedAt = DateTime.UtcNow;
        }

        public void UpdateVote(VoteType newType)
        {
            Type = newType;
            VotedAt = DateTime.UtcNow;
        }
    }
}
