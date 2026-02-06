namespace Explorer.Payments.API.Dtos;

public class CouponValidationDto
{
    public bool IsValid { get; set; }
    public string Code { get; set; }
    public int DiscountPercent { get; set; }
    public double OriginalTotal { get; set; }
    public double DiscountedTotal { get; set; }
    public double Savings { get; set; }
    public string Message { get; set; }
}
