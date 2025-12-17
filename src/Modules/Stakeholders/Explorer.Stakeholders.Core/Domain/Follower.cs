namespace Explorer.Stakeholders.Core.Domain
{
    public class Follower
    {
        public long UserId { get; private set; }
        public long FollowedUserId { get; private set; }
        public DateTime FollowedAt { get; private set; }

        private Follower()
        {
        }

        public Follower(long userId, long followedUserId)
        {
            UserId = userId;
            FollowedUserId = followedUserId;
            FollowedAt = DateTime.UtcNow;
            Validate();
        }

        private void Validate()
        {
            if (UserId == 0) throw new ArgumentException("Invalid UserId");
            if (FollowedUserId == 0) throw new ArgumentException("Invalid FollowedUserId");
            if (UserId == FollowedUserId) throw new ArgumentException("User cannot follow themselves");
        }
    }
}
