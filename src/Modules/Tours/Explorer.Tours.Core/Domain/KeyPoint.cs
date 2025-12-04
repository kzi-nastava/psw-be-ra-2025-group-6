using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

// TODO: Align with full KeyPoints implementation when available.
public class KeyPoint : Entity
{
    public long TourId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    protected KeyPoint() { }

    public KeyPoint(long tourId, double latitude, double longitude)
    {
        if (latitude is < -90 or > 90) throw new ArgumentException("Latitude out of range.");
        if (longitude is < -180 or > 180) throw new ArgumentException("Longitude out of range.");
        TourId = tourId;
        Latitude = latitude;
        Longitude = longitude;
    }
}
