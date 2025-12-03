using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Tour : AggregateRoot
{
    public string Name { get; init; }
    public string? Description { get; init; }
    public TourDifficulty Difficulty { get; init; }
    public List<string>? Tags { get; init; }
    public float Price { get; init; }
    public TourStatus Status { get; init; }

    public long AuthorId { get; init; }

    public List<Equipment>? Equipment { get; private set; }

    private Tour() {
    }

    public Tour(
        string name,
        string description,
        TourDifficulty difficulty,
        List<string> tags,
        float price,
        TourStatus status,
        List<Equipment> equipment)
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
}
