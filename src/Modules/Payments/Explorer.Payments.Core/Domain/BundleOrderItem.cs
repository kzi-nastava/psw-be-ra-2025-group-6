using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class BundleOrderItem : Entity
{
    public long BundleId { get; init; }
    public string BundleName { get; init; }
    public double Price { get; init; }

    public BundleOrderItem(long bundleId, string bundleName, double price)
    {
        BundleId = bundleId;
        BundleName = bundleName;
        Price = price;
        Validate();
    }

    private void Validate()
    {
        if (BundleId == 0) throw new ArgumentException("Invalid BundleId");
        if (string.IsNullOrWhiteSpace(BundleName)) throw new ArgumentException("Invalid BundleName");
        if (Price < 0) throw new ArgumentException("Invalid Price");
    }
}
