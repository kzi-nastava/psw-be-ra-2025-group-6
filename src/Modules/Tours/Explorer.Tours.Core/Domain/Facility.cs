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

        public Facility(string name, string? comment, double longitude, double latitude, FacilityType type)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name.");
            Name = name;
            if (string.IsNullOrWhiteSpace(comment))
                Comment = "";
            else Comment = comment;

            if (!(longitude >= 0 || longitude <= 180)) throw new ArgumentException("Invalid Longitude.");
            if (!(latitude >= 0 || latitude <= 180)) throw new ArgumentException("Invalid Latitude.");
            if (!(type == FacilityType.Toilet || type == FacilityType.Other || type == FacilityType.Parking || type == FacilityType.Restaurant)) throw new ArgumentException("Invalid FacilityType.");
        }
    }
}
