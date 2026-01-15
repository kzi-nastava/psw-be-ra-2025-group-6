namespace Explorer.Payments.API.Dtos;

public class PaymentRecordDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long? TourId { get; set; }
    public long? BundleId { get; set; }
    public double OriginalPrice { get; set; }
    public double FinalPrice { get; set; }
    public int? DiscountPercent { get; set; }
    public string? CouponCode { get; set; }
    public DateTime PurchaseTime { get; set; }
}
