namespace Explorer.Payments.API.Dtos;

public class ShoppingCartDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public List<BundleOrderItemDto> BundleItems { get; set; } = new();
    public double TotalPrice { get; set; }
}
