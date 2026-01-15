namespace Explorer.Payments.API.Dtos;

public class CouponDto
{
    public long Id { get; set; }
    public string Code { get; set; }
    public int DiscountPercent { get; set; }
    public DateTime? ValidUntil { get; set; }
    public long? TourId { get; set; }
    public long AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCouponDto
{
    public int DiscountPercent { get; set; }
    public long? TourId { get; set; }
    public DateTime? ValidUntil { get; set; }
}
