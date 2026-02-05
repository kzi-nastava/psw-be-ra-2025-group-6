using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubMembershipRequest : Entity
    {
        public long ClubId { get; private set; }
        public long TouristId { get; private set; }
        public DateTime RequestedAt { get; private set; }
        public ClubMembershipRequestStatus Status { get; private set; }

        public ClubMembershipRequest(long clubId, long touristId)
        {
            ClubId = clubId;
            TouristId = touristId;
            RequestedAt = DateTime.UtcNow;
            Status = ClubMembershipRequestStatus.Processing;
            Validate();
        }

        private void Validate()
        {
            if (ClubId == 0) throw new ArgumentException("Invalid ClubId");
            if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
        }
    }

    public enum ClubMembershipRequestStatus
    {
        Processing = 0,
        Accepted = 1,
        Rejected = 2
    }
}
