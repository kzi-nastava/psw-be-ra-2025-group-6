using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public enum ChallengeStatus { Draft, Active, Archived }
    public enum ChallengeType { Social, Location, Misc }

    public class Challenge : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public int XP { get; private set; }
        public ChallengeStatus Status { get; private set; }
        public ChallengeType Type { get; private set; }
        public long? CreatorId { get; private set; }
        public bool IsCreatedByTourist { get; private set; }

        private Challenge() { }

        public Challenge(string title, string description, double longitude, double latitude, int xp, ChallengeType type, ChallengeStatus status = ChallengeStatus.Draft)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Invalid Title.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");
            if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude.");
            if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude.");
            if (xp < 0) throw new ArgumentException("XP must be non-negative.");

            Title = title;
            Description = description;
            Longitude = longitude;
            Latitude = latitude;
            XP = xp;
            Type = type;
            Status = status;
            IsCreatedByTourist = false;
        }

        // Constructor for tourist-created challenges
        public Challenge(string title, string description, double longitude, double latitude, int xp, ChallengeType type, long creatorId)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Invalid Title.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");
            if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude.");
            if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude.");
            if (xp < 0) throw new ArgumentException("XP must be non-negative.");
            if (creatorId <= 0) throw new ArgumentException("Invalid CreatorId.");

            Title = title;
            Description = description;
            Longitude = longitude;
            Latitude = latitude;
            XP = xp;
            Type = type;
            Status = ChallengeStatus.Draft; // Tourist-created challenges start as Draft
            CreatorId = creatorId;
            IsCreatedByTourist = true;
        }

        public void Publish()
        {
            if (Status == ChallengeStatus.Archived) throw new InvalidOperationException("Cannot publish archived challenge.");
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Description)) throw new InvalidOperationException("Challenge must have title and description to be published.");
            Status = ChallengeStatus.Active;
        }

        public void Archive()
        {
            Status = ChallengeStatus.Archived;
        }

        public void Update(string title, string description, double longitude, double latitude, int xp, ChallengeType type)
        {
            if (Status == ChallengeStatus.Archived) throw new InvalidOperationException("Cannot modify archived challenge.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Invalid Title.");

            Title = title;
            Description = description;
            if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude.");
            if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude.");
            Longitude = longitude;
            Latitude = latitude;
            if (xp < 0) throw new ArgumentException("XP must be non-negative.");
            XP = xp;
            Type = type;
        }

        // Allow admin to change status via service. Uses existing domain rules where applicable.
        public void SetStatus(ChallengeStatus status)
        {
            if (status == ChallengeStatus.Active)
            {
                Publish();
                return;
            }

            if (status == ChallengeStatus.Archived)
            {
                Archive();
                return;
            }

            // status == Draft
            if (Status == ChallengeStatus.Archived)
                throw new InvalidOperationException("Cannot set status of an archived challenge back to Draft.");

            Status = ChallengeStatus.Draft;
        }
    }
}
