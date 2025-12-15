using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class ShoppingCart : Entity
{
    public long TouristId { get; init; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items;
    public double TotalPrice => _items.Sum(item => item.Price);
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

        if (_items.Any(item => item.TourId == tour.Id))
            throw new ArgumentException("Tour is already in the cart.");

        var orderItem = new OrderItem(tour.Id, tour.Name, tour.Price);
        _items.Add(orderItem);
        LastModified = DateTime.UtcNow;
    }

    public void RemoveItem(long tourId)
    {
        var itemToRemove = _items.FirstOrDefault(item => item.TourId == tourId);
        if (itemToRemove != null)
        {
            _items.Remove(itemToRemove);
            LastModified = DateTime.UtcNow;
        }
    }

    public List<TourPurchaseToken> Checkout()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot checkout an empty cart.");

        var tokens = _items
            .Select(item => new TourPurchaseToken(TouristId, item.TourId, item.TourName, item.Price))
            .ToList();

        _items.Clear();
        LastModified = DateTime.UtcNow;

        return tokens;
    }
}
