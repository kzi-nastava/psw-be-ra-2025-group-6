using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System.Collections.Specialized;

namespace Explorer.Tours.Core.Domain;

public class Tour : AggregateRoot
{
    public string Name { get; init; }
    public string? Description { get; init; }
    public TourDifficulty Difficulty { get; init; }
    public List<string>? Tags { get; init; }
    public float Price { get; init; }
    public TourStatus Status { get; private set; }

    public long AuthorId { get; init; }

    public List<Equipment>? Equipment { get; private set; }

    public List<KeyPoint>? KeyPoints { get; private set; }

    public double DistanceInKm { get; private set; }

    public List<TourDuration>? Duration { get; private set; }
    public DateTime? PublishedTime { get; private set; }

    private Tour() {
    }

    public Tour(
        string name,
        string description,
        TourDifficulty difficulty,
        List<string> tags,
        float price,
        TourStatus status,
        List<Equipment> equipment,
        List<KeyPoint> keypoints,
        List<TourDuration> durations)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid Name.");

        if (float.IsNegative(price))
            throw new ArgumentException("Invalid Price.");
        if (!Enum.IsDefined(typeof(TourDifficulty), difficulty))
            throw new ArgumentException("Invalid Difficulty.");

        if (!Enum.IsDefined(typeof(TourStatus), status))
            throw new ArgumentException("Invalid Status.");


        Name = name;
        Description = description;
        Difficulty = difficulty;
        Tags = tags ?? new List<string>();
        Price = price;
        Status = status;
        Equipment = equipment ?? new List<Equipment>();

        KeyPoints = keypoints ?? new List<KeyPoint>();
        DistanceInKm = 0;
        Duration = durations ?? new List<TourDuration>();


    }
    public Tour(
        string name,
        string description,
        TourDifficulty difficulty,
        List<string> tags,
        long AuthorId)
    {

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid Name.");

        if (!Enum.IsDefined(typeof(TourDifficulty), difficulty))
            throw new ArgumentException("Invalid Difficulty.");
        if (AuthorId <= 0)
            throw new ArgumentException("Invalid AuthorId.");

   


        Name = name;
        Description = description;
        Difficulty = difficulty;
        Tags = tags ?? new List<string>();
        Price = 0;
        Status = TourStatus.DRAFT;
        this.AuthorId = AuthorId;
        Equipment = new List<Equipment>();

        KeyPoints = new List<KeyPoint>();
        DistanceInKm = 0;
        Duration = new List<TourDuration>();
    }
    public void AddEquipment(Equipment equipment)
    {
        if (equipment == null) { 
        throw new ArgumentException("Invalid Equipment.");
        }
        if (Status == TourStatus.ARCHIVED)
        {
            throw new InvalidOperationException("Cannot modify equipment of an archived tour.");
        }
        else
        {
            Equipment.Add(equipment);
        }
    }
    public void RemoveEquipment(Equipment equipment) {

        if (equipment == null)
        {
            throw new ArgumentException("Invalid Equipment.");
        }
        if (Status == TourStatus.ARCHIVED)
        {
            throw new InvalidOperationException("Cannot modify equipment of an archived tour.");
        }
        else
        {
            Equipment.Remove(equipment);
        }
    }

    public void Archive(long authorId)
    {
        if (AuthorId != authorId)
            throw new ForbiddenException("Only the owner can archive the tour.");
        if (Status != TourStatus.CONFIRMED)
            throw new InvalidOperationException("Tour must be published to be archived.");

        Status = TourStatus.ARCHIVED;
    }

    public void Activate(long authorId)
    {
        if (AuthorId != authorId)
            throw new ForbiddenException("Only the owner can activate the tour.");
        if (Status != TourStatus.ARCHIVED)
            throw new InvalidOperationException("Tour must be archived to be published.");

        Status = TourStatus.CONFIRMED;
    }

    public void AddKeyPoint(KeyPoint keyPoint)
    {
        if (keyPoint == null) throw new ArgumentException("Invalid KeyPoint.");
        if (Status == TourStatus.ARCHIVED)
            throw new InvalidOperationException("Cannot modify key points of an archived tour.");

        KeyPoints.Add(keyPoint);
    }

    /// <summary>
    /// Returns the first key point as the reference point for distance calculations.
    /// This keeps distance search consistent when tours only store coordinates on key points.
    /// </summary>
    public KeyPoint? GetReferenceKeyPoint()
    {
        return KeyPoints?.FirstOrDefault();
    }
    public void SetDistance(double distance)
    {
        if (distance < 0) throw new ArgumentException("Distance cannot be negative.");
        DistanceInKm = distance;
    }
    public void SetDuration(TourDuration duration)
    {
        if (Status == TourStatus.ARCHIVED)
            throw new InvalidOperationException("Cannot modify duration of archived tour.");

        if (duration == null)
            throw new ArgumentException("Invalid duration.");

        var existing = Duration.FirstOrDefault(d => d.TravelType == duration.TravelType);
        if (existing != null)
        {
            existing.UpdateMinutes(duration.Minutes);
        }
        else
        {
            Duration.Add(duration);
        }
    }
    

    public bool IsWithinRadius(double centerLat, double centerLon, double radiusInKm)
    {
        var referencePoint = GetReferenceKeyPoint();
        if (referencePoint == null) return false;

        var distance = HaversineDistanceInKm(centerLat, centerLon, referencePoint.Latitude, referencePoint.Longitude);
        return distance <= radiusInKm;
    }

    public double DistanceTo(double centerLat, double centerLon)
    {
        var referencePoint = GetReferenceKeyPoint();
        if (referencePoint == null) return double.PositiveInfinity;

        return HaversineDistanceInKm(centerLat, centerLon, referencePoint.Latitude, referencePoint.Longitude);
    }

    private static double HaversineDistanceInKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
    public KeyPoint? GetFirstKeyPoint()
    {
        return KeyPoints.FirstOrDefault();
    }
    public void Publish(long authorId)
    {
        if (AuthorId != authorId)
            throw new ForbiddenException("Only the owner can publish the tour.");

        if (Status != TourStatus.DRAFT)
            throw new InvalidOperationException("Only draft tours can be published.");

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) || Price < 0)
            throw new InvalidOperationException("Tour must have all basic fields filled.");

        if (Tags == null || Tags.Count == 0)
            throw new InvalidOperationException("Tour must have at least one tag.");

        if (KeyPoints == null || KeyPoints.Count < 2)
            throw new InvalidOperationException("Tour must have at least two key points.");

        Status = TourStatus.CONFIRMED; 
        PublishedTime = DateTime.UtcNow;
    }

    private static double ToRadians(double angle) => Math.PI * angle / 180.0;
}
