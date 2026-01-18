using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class KeyPoint : Entity
{
    public long TourId { get; init; }
    public string Name { get; init; }

    public string Description { get; init; }
    public double Longitude { get; init; }
    public double Latitude { get; init; }
    public string ImagePath { get; init; }
    public string Secret { get; init; }
    public bool IsPublic { get; private set; }
    public long? PublicRequestId { get; private set; }

    public KeyPoint(long tourId, string name, string description, double longitude, double latitude, string imagePath, string secret)
    {
        if (tourId == 0) throw new ArgumentException("Invalid TourId");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name");
        if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude");
        if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude");
        if (string.IsNullOrWhiteSpace(imagePath)) throw new ArgumentException("Invalid ImagePath");
        if (string.IsNullOrWhiteSpace(secret)) throw new ArgumentException("Invalid Secret");

        TourId = tourId;
        Name = name;
        Description = description;
        Longitude = longitude;
        Latitude = latitude;
        ImagePath = imagePath;
        Secret = secret;
        IsPublic = false;
    }

    public void MarkAsPublicRequested(long requestId)
    {
        if (PublicRequestId.HasValue)
            throw new InvalidOperationException("Public request already exists for this keypoint.");

        PublicRequestId = requestId;
    }

    public void ApprovePublic()
    {
        if (!PublicRequestId.HasValue)
            throw new InvalidOperationException("No public request exists for this keypoint.");

        IsPublic = true;
    }
}