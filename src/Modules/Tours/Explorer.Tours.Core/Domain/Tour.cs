using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Tour : Entity
{
    public string Name { get; init; }
    public string? Description { get; init; }
    public TourDifficulty Difficulty { get; init; }
    public List<string>? Tags { get; init; }
    public float Price { get; init; }
    public TourStatus Status { get; init; }
    public ICollection<KeyPoint> KeyPoints { get; init; } = new List<KeyPoint>();

    public Tour(
        string name,
        string description,
        TourDifficulty difficulty,
        List<string> tags,
        float price,
        TourStatus status)
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
    }
}
