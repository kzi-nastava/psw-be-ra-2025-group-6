using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> ImageUris { get; private set; }
        public long OwnerId { get; private set; } 

        public Club(string name, string description, List<string> imageUris, long ownerId)
        {
            Name = name;
            Description = description;
            ImageUris = imageUris;
            OwnerId = ownerId;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name.");
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description.");
            if (OwnerId == 0) throw new ArgumentException("Invalid OwnerId.");
            if (ImageUris == null || !ImageUris.Any()) throw new ArgumentException("At least one image URI is required.");
        }
    }
}