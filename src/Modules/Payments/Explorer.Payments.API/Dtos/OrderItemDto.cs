namespace Explorer.Payments.API.Dtos;

public class OrderItemDto
{
    public long TourId { get; set; }
    public string TourName { get; set; }
    public double Price { get; set; }
    public double? OriginalPrice { get; set; }
    public double? DiscountedPrice { get; set; }
    public int? DiscountPercent { get; set; }
    public bool IsOnSale { get; set; }
    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }
}
