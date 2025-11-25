using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Meetup : Entity
{
    public string Name { get; init; }
    public string Description { get; init; }
    public DateTime EventDate { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public long CreatorId { get; init; }
    public DateTime LastModified { get; init; }

    public Meetup(string name, string description, DateTime eventDate, double latitude, double longitude, long creatorId, DateTime lastModified)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");
        if (latitude is < -90 or > 90) throw new ArgumentException("Invalid Latitude.");
        if (longitude is < -180 or > 180) throw new ArgumentException("Invalid Longitude.");
        if (creatorId == 0) throw new ArgumentException("Invalid CreatorId.");

        Name = name;
        Description = description;
        EventDate = eventDate;
        Latitude = latitude;
        Longitude = longitude;
        CreatorId = creatorId;
        LastModified = lastModified;
    }
}
