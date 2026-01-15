using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public enum BundleStatus
{
    DRAFT,
    PUBLISHED,
    ARCHIVED
}

public class Bundle : Entity
{
    public long AuthorId { get; init; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public BundleStatus Status { get; private set; }
    
    private readonly List<long> _tourIds = new();
    public IReadOnlyCollection<long> TourIds => _tourIds.AsReadOnly();
    
    public DateTime CreatedAt { get; init; }
    public DateTime? PublishedAt { get; private set; }

    private Bundle() { }

    public Bundle(long authorId, string name, double price, List<long> tourIds)
    {
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Bundle name is required");
        if (price < 0) throw new ArgumentException("Price cannot be negative");
        if (tourIds == null || tourIds.Count == 0) throw new ArgumentException("Bundle must contain at least one tour");

        AuthorId = authorId;
        Name = name;
        Price = price;
        _tourIds = tourIds;
        Status = BundleStatus.DRAFT;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, double price, List<long> tourIds)
    {
        if (Status != BundleStatus.DRAFT)
            throw new InvalidOperationException("Only draft bundles can be modified");

        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Bundle name is required");
        if (price < 0) throw new ArgumentException("Price cannot be negative");
        if (tourIds == null || tourIds.Count == 0) throw new ArgumentException("Bundle must contain at least one tour");

        Name = name;
        Price = price;
        _tourIds.Clear();
        _tourIds.AddRange(tourIds);
    }

    public void Publish(int publishedToursCount)
    {
        if (Status != BundleStatus.DRAFT)
            throw new InvalidOperationException("Only draft bundles can be published");

        if (publishedToursCount < 2)
            throw new InvalidOperationException("Bundle must contain at least 2 published tours");

        Status = BundleStatus.PUBLISHED;
        PublishedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status != BundleStatus.PUBLISHED)
            throw new InvalidOperationException("Only published bundles can be archived");

        Status = BundleStatus.ARCHIVED;
    }
}
