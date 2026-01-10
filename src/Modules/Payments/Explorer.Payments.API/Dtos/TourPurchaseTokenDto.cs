namespace Explorer.Payments.API.Dtos;

public class TourPurchaseTokenDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long TourId { get; set; }
    public string TourName { get; set; }
    public double Price { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool IsUsed { get; set; }
}
