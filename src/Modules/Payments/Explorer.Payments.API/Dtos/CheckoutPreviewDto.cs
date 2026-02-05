namespace Explorer.Payments.API.Dtos;

public class CheckoutPreviewDto
{
    public double OriginalTotalPrice { get; set; }
    public double FinalTotalPrice { get; set; }
    public double TotalDiscount { get; set; }
    public int? DiscountPercent { get; set; }
    public string? CouponCode { get; set; }
    public bool HasDiscount { get; set; }
    public List<CheckoutItemPreviewDto> Items { get; set; } = new();
}

public class CheckoutItemPreviewDto
{
    public long TourId { get; set; }
    public string TourName { get; set; }
    public double OriginalPrice { get; set; }
    public double FinalPrice { get; set; }
    public int? DiscountPercent { get; set; }
    public bool HasDiscount { get; set; }
}
