using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Monument : Entity
    {
        public string Name { get; init; }
        public string? Description { get; init; }
        public int? YearOfOrigin { get; init; }
        public string Status { get; private set; } = "active";
        public double LocationLatitude { get; init; }
        public double LocationLongitude { get; init; }

        public Monument(string name, double locationLatitude, double locationLongitude, string? description = null, int? yearOfOrigin = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid Name.");

            if (locationLatitude < -90 || locationLatitude > 90)
                throw new ArgumentException("Invalid LocationLatitude.");

            if (locationLongitude < -180 || locationLongitude > 180)
                throw new ArgumentException("Invalid LocationLongitude.");

            if (yearOfOrigin.HasValue && yearOfOrigin.Value > DateTime.UtcNow.Year)
                throw new ArgumentException("YearOfOrigin cannot be in the future.");

            Name = name;
            Description = description;
            YearOfOrigin = yearOfOrigin;
            LocationLatitude = locationLatitude;
            LocationLongitude = locationLongitude;
            Status = "active";
        }

        public void SetStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Invalid Status.");
            Status = status;
        }
    }
}