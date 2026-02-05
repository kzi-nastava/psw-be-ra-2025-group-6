namespace Explorer.Payments.API.Dtos;

public class BundleOrderItemDto
{
    public long BundleId { get; set; }
    public string BundleName { get; set; }
    public double Price { get; set; }
}
