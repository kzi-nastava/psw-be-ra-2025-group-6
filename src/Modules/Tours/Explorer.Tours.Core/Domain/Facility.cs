using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public enum FacilityType { Toilet, Restaurant, Parking, Other }
    public class Facility : Entity
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public FacilityType Type { get; set; }
        public bool IsPublic { get; private set; }
        public long? PublicRequestId { get; private set; }

        public Facility(string name, string? comment, double longitude, double latitude, FacilityType type)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name.");
            Name = name;
            if (string.IsNullOrWhiteSpace(comment))
                Comment = "";
            else Comment = comment;

            if (!(longitude >= -180 && longitude <= 180)) throw new ArgumentException("Invalid Longitude."); else Longitude = longitude;
            if (!(latitude >= -90 && latitude <= 90)) throw new ArgumentException("Invalid Latitude."); else Latitude = latitude;
            if (!(type == FacilityType.Toilet || type == FacilityType.Other || type == FacilityType.Parking || type == FacilityType.Restaurant)) throw new ArgumentException("Invalid FacilityType.");
            else Type = type;
            IsPublic = false;
        }

        public void MarkAsPublicRequested(long requestId)
        {
            if (PublicRequestId.HasValue)
                throw new InvalidOperationException("Public request already exists for this facility.");

            PublicRequestId = requestId;
        }

        public void ApprovePublic()
        {
            if (!PublicRequestId.HasValue)
                throw new InvalidOperationException("No public request exists for this facility.");

            IsPublic = true;
        }
    }
}
