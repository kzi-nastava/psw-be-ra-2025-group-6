namespace Explorer.Payments.API.Dtos;

public class PaymentRecordDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long TourId { get; set; }
    public float Price { get; set; }
    public DateTime PurchasedAt { get; set; }
}
