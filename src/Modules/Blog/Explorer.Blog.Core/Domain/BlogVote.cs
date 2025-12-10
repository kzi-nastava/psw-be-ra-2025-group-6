using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogVote : ValueObject
    {
        public long UserId { get; private set; }
        public DateTime VotedAt { get; private set; }
        public VoteType Type { get; private set; }

        private BlogVote() { }

        public BlogVote(long userId, VoteType type)
        {
            if (userId == 0) throw new ArgumentException("Invalid UserId.");
            UserId = userId;
            Type = type;
            VotedAt = DateTime.UtcNow;
        }

        public void UpdateVote(VoteType newType)
        {
            Type = newType;
            VotedAt = DateTime.UtcNow;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserId;
            yield return Type;
        }
    }
}
