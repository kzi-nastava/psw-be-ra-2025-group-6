using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class PaymentRecord : AggregateRoot
{
    public long TouristId { get; init; }
    public long TourId { get; init; }
    public float Price { get; init; }
    public DateTime PurchasedAt { get; init; }

    public PaymentRecord(long touristId, long tourId, float price, DateTime purchasedAt)
    {
        TouristId = touristId;
        TourId = tourId;
        Price = price;
        PurchasedAt = purchasedAt;
        Validate();
    }

    private void Validate()
    {
        if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
        if (TourId == 0) throw new ArgumentException("Invalid TourId");
        if (Price < 0) throw new ArgumentException("Invalid Price");
    }
}
