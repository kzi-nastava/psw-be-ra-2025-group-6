using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class ShoppingCart : Entity
{
    public long TouristId { get; init; }
    private readonly List<OrderItem> _items = new();
    private readonly List<BundleOrderItem> _bundleItems = new();
    
    public IReadOnlyCollection<OrderItem> Items => _items;
    public IReadOnlyCollection<BundleOrderItem> BundleItems => _bundleItems;
    
    public double TotalPrice => _items.Sum(item => item.Price) + _bundleItems.Sum(item => item.Price);
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

    public void AddItem(long tourId, string tourName, double price, string tourStatus)
    {
        if (tourStatus != "CONFIRMED")
            throw new ArgumentException("Tour must be confirmed to be purchased.");

        if (_items.Any(item => item.TourId == tourId))
            throw new ArgumentException("Tour is already in the cart.");

        var orderItem = new OrderItem(tourId, tourName, price);
        _items.Add(orderItem);
        LastModified = DateTime.UtcNow;
    }

    public void AddBundle(long bundleId, string bundleName, double price, string bundleStatus)
    {
        if (bundleStatus != "PUBLISHED")
            throw new ArgumentException("Bundle must be published to be purchased.");

        if (_bundleItems.Any(item => item.BundleId == bundleId))
            throw new ArgumentException("Bundle is already in the cart.");

        var bundleItem = new BundleOrderItem(bundleId, bundleName, price);
        _bundleItems.Add(bundleItem);
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

    public void RemoveBundle(long bundleId)
    {
        var itemToRemove = _bundleItems.FirstOrDefault(item => item.BundleId == bundleId);
        if (itemToRemove != null)
        {
            _bundleItems.Remove(itemToRemove);
            LastModified = DateTime.UtcNow;
        }
    }

    public List<TourPurchaseToken> Checkout()
    {
        if (_items.Count == 0 && _bundleItems.Count == 0)
            throw new InvalidOperationException("Cannot checkout an empty cart.");

        var tokens = _items
            .Select(item => new TourPurchaseToken(TouristId, item.TourId, item.TourName, item.Price))
            .ToList();

        _items.Clear();
        _bundleItems.Clear();
        LastModified = DateTime.UtcNow;

        return tokens;
    }
}
