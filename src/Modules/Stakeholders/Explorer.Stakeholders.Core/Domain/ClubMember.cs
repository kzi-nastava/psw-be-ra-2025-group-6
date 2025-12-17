namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubMember
    {
        public long UserId { get; private set; }
        public long ClubId { get; private set; }
        public DateTime JoinedAt { get; private set; }

        private ClubMember()
        {
        }

        public ClubMember(long userId, long clubId)
        {
            UserId = userId;
            ClubId = clubId;
            JoinedAt = DateTime.UtcNow;
            Validate();
        }

        private void Validate()
        {
            if (UserId == 0) throw new ArgumentException("Invalid UserId");
            if (ClubId == 0) throw new ArgumentException("Invalid ClubId");
        }
    }
}
