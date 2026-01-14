using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class Sale : Entity
{
    public long AuthorId { get; init; }
    public int DiscountPercent { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    
    private readonly List<long> _tourIds = new();
    public IReadOnlyCollection<long> TourIds => _tourIds.AsReadOnly();

    private Sale() { }

    public Sale(long authorId, List<long> tourIds, DateTime startDate, DateTime endDate, int discountPercent)
    {
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId");
        if (tourIds == null || tourIds.Count == 0) throw new ArgumentException("Sale must include at least one tour");
        if (discountPercent < 1 || discountPercent > 100)
            throw new ArgumentException("Discount percent must be between 1 and 100");
        if (startDate >= endDate) throw new ArgumentException("Start date must be before end date");
        
        var duration = (endDate - startDate).Days;
        if (duration > 14) throw new ArgumentException("Sale cannot last more than 2 weeks (14 days)");

        AuthorId = authorId;
        _tourIds = tourIds;
        StartDate = startDate;
        EndDate = endDate;
        DiscountPercent = discountPercent;
    }

    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        return now >= StartDate && now <= EndDate;
    }

    public bool AppliesToTour(long tourId)
    {
        return _tourIds.Contains(tourId);
    }
}
