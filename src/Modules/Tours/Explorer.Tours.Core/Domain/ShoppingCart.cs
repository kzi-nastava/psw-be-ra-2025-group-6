using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class ShoppingCart : Entity
{
    public long TouristId { get; init; }
    public List<OrderItem> Items { get; init; } = new List<OrderItem>();
    public double TotalPrice => Items.Sum(item => item.Price);
    public DateTime LastModified { get; private set; }

    public ShoppingCart(long touristId)
    {
        TouristId = touristId;
        LastModified = DateTime.UtcNow;
        Validate();
    }

    private void Validate()
    {
        if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
    }

    public void AddItem(Tour tour)
    {
        if (tour.Status != TourStatus.CONFIRMED)
            throw new ArgumentException("Tour must be confirmed to be purchased.");

        if (Items.Any(item => item.TourId == tour.Id))
            throw new ArgumentException("Tour is already in the cart.");

        var orderItem = new OrderItem(tour.Id, tour.Name, tour.Price);
        Items.Add(orderItem);
        LastModified = DateTime.UtcNow;
    }

    public void RemoveItem(long tourId)
    {
        var itemToRemove = Items.FirstOrDefault(item => item.TourId == tourId);
        if (itemToRemove != null)
        {
            Items.Remove(itemToRemove);
            LastModified = DateTime.UtcNow;
        }
    }
}
