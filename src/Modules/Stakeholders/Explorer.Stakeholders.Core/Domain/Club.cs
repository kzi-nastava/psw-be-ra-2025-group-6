using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> ImageUris { get; private set; }
        public long OwnerId { get; private set; }
        public ClubStatus Status { get; private set; }

        public Club(string name, string description, List<string> imageUris, long ownerId, ClubStatus status = ClubStatus.Active)
        {
            Name = name;
            Description = description;
            ImageUris = imageUris;
            OwnerId = ownerId;
            Status = status;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name.");
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description.");
            if (OwnerId == 0) throw new ArgumentException("Invalid OwnerId.");
            if (ImageUris == null || !ImageUris.Any()) throw new ArgumentException("At least one image URI is required.");
        }

        public void Update(string name, string description, List<string> imageUris, ClubStatus status)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");
            if (imageUris == null || !imageUris.Any()) throw new ArgumentException("At least one image URI is required.");

            Name = name;
            Description = description;
            ImageUris = imageUris;
            Status = status;
        }

        public void ChangeStatus(ClubStatus newStatus, long requestingUserId)
        {
            if (requestingUserId != OwnerId)
                throw new UnauthorizedAccessException("Only the owner can change club status");

            Status = newStatus;
        }

        public bool IsOwner(long userId)
        {
            return OwnerId == userId;
        }

        public bool CanAcceptMembers()
        {
            return Status == ClubStatus.Active;
        }
    }
}