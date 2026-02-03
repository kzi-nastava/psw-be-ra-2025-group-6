using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubMember : Entity
    {
        public long ClubId { get; private set; }
        public long UserId { get; private set; }
        public DateTime JoinedAt { get; private set; }
        public ClubMemberStatus Status { get; private set; }

        private ClubMember() { }

        public ClubMember(long clubId, long userId)
        {
            ClubId = clubId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
            Status = ClubMemberStatus.Active;
            Validate();
        }

        private void Validate()
        {
            if (ClubId == 0) throw new ArgumentException("Invalid ClubId");
            if (UserId == 0) throw new ArgumentException("Invalid UserId");
        }

        public void Remove()
        {
            Status = ClubMemberStatus.Removed;
        }
    }

    public enum ClubMemberStatus
    {
        Active = 0,
        Removed = 1
    }
}
