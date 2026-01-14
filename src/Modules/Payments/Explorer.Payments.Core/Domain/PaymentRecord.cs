using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain;

public class PaymentRecord : Entity
{
    public long TouristId { get; init; }
    public long? TourId { get; init; }
    public long? BundleId { get; init; }
    public double OriginalPrice { get; init; }
    public double FinalPrice { get; init; }
    public int? DiscountPercent { get; init; }
    public string? CouponCode { get; init; }
    public DateTime PurchaseTime { get; init; }

    private PaymentRecord() { }

    public PaymentRecord(long touristId, long? tourId, long? bundleId, double originalPrice, double finalPrice, int? discountPercent = null, string? couponCode = null)
    {
        if (touristId == 0) throw new ArgumentException("Invalid TouristId");
        if (!tourId.HasValue && !bundleId.HasValue)
            throw new ArgumentException("Either TourId or BundleId must be provided");
        if (originalPrice < 0) throw new ArgumentException("Original price cannot be negative");
        if (finalPrice < 0) throw new ArgumentException("Final price cannot be negative");

        TouristId = touristId;
        TourId = tourId;
        BundleId = bundleId;
        OriginalPrice = originalPrice;
        FinalPrice = finalPrice;
        DiscountPercent = discountPercent;
        CouponCode = couponCode;
        PurchaseTime = DateTime.UtcNow;
    }
}
