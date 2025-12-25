using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class TourPurchaseToken : Entity
{
    public long TouristId { get; init; }
    public long TourId { get; init; }
    public string TourName { get; init; }
    public double Price { get; init; }
    public DateTime PurchaseDate { get; init; }
    public bool IsUsed { get; private set; }

    public TourPurchaseToken(long touristId, long tourId, string tourName, double price)
    {
        TouristId = touristId;
        TourId = tourId;
        TourName = tourName;
        Price = price;
        PurchaseDate = DateTime.UtcNow;
        IsUsed = false;
        Validate();
    }

    private void Validate()
    {
        if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
        if (TourId == 0) throw new ArgumentException("Invalid TourId");
        if (string.IsNullOrWhiteSpace(TourName)) throw new ArgumentException("Invalid TourName");
        if (Price < 0) throw new ArgumentException("Invalid Price");
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}
