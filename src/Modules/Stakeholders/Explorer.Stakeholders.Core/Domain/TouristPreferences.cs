using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class TouristPreferences : Entity
{
    public const int MaxTags = 20;
    public const int MaxTagLength = 50;
    public const int MinRating = 0;
    public const int MaxRating = 3;

    public long TouristId { get; private set; }
    public int? PreferredDifficulty { get; private set; }
    public int WalkRating { get; private set; }
    public int BikeRating { get; private set; }
    public int CarRating { get; private set; }
    public int BoatRating { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private TouristPreferences() { }

    public TouristPreferences(
        long touristId,
        int? preferredDifficulty,
        int walkRating,
        int bikeRating,
        int carRating,
        int boatRating,
        List<string>? tags)
    {
        TouristId = touristId;
        PreferredDifficulty = preferredDifficulty;
        WalkRating = walkRating;
        BikeRating = bikeRating;
        CarRating = carRating;
        BoatRating = boatRating;
        Tags = tags ?? new List<string>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    public void Update(
        int? preferredDifficulty,
        int walkRating,
        int bikeRating,
        int carRating,
        int boatRating,
        List<string>? tags)
    {
        PreferredDifficulty = preferredDifficulty;
        WalkRating = walkRating;
        BikeRating = bikeRating;
        CarRating = carRating;
        BoatRating = boatRating;
        Tags = tags ?? new List<string>();
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    private void Validate()
    {
        if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
        ValidateRating(WalkRating, nameof(WalkRating));
        ValidateRating(BikeRating, nameof(BikeRating));
        ValidateRating(CarRating, nameof(CarRating));
        ValidateRating(BoatRating, nameof(BoatRating));

        if (Tags.Count > MaxTags) throw new ArgumentException($"Tags count cannot exceed {MaxTags}.");
        foreach (var tag in Tags)
        {
            if (string.IsNullOrWhiteSpace(tag)) throw new ArgumentException("Tag cannot be empty.");
            if (tag.Length > MaxTagLength) throw new ArgumentException($"Tag length cannot exceed {MaxTagLength}.");
        }
    }

    private static void ValidateRating(int rating, string name)
    {
        if (rating < MinRating || rating > MaxRating)
            throw new ArgumentException($"{name} must be between {MinRating} and {MaxRating}.");
    }
}
