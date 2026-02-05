namespace Explorer.Payments.API.Dtos;

public class SaleDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public int DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<long> TourIds { get; set; }
}

public class CreateSaleDto
{
    public List<long> TourIds { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DiscountPercent { get; set; }
}
