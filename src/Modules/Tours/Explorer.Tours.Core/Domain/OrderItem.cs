using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class OrderItem : Entity
{
    public long TourId { get; init; }
    public string TourName { get; init; }
    public double Price { get; init; }

    public OrderItem(long tourId, string tourName, double price)
    {
        TourId = tourId;
        TourName = tourName;
        Price = price;
        Validate();
    }

    private void Validate()
    {
        if (TourId == 0) throw new ArgumentException("Invalid TourId");
        if (string.IsNullOrWhiteSpace(TourName)) throw new ArgumentException("Invalid TourName");
        if (Price < 0) throw new ArgumentException("Invalid Price");
    }
}
