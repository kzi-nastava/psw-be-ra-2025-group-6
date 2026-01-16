namespace Explorer.Tours.API.Dtos;

public class TourDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TourDifficultyDto Difficulty { get; set; }
    public List<string>? Tags { get; set; }
    public float Price { get; set; }
    public TourStatusDto Status { get; set; }
    public long AuthorId { get; set; }

    public List<EquipmentDto>? Equipment { get; set; }
    public List<KeyPointDto>? KeyPoints { get; set; } = new List<KeyPointDto>();
    public double DistanceInKm { get; set; }
    public List<TourDurationDto>? Duration { get; set;  }
    public DateTime? PublishedTime { get; set; }
    
    // Sale/Discount information
    public double? OriginalPrice { get; set; }
    public double? DiscountedPrice { get; set; }
    public int? DiscountPercent { get; set; }
    public bool IsOnSale { get; set; }
    public DateTime? SaleStartDate { get; set; }
    public DateTime? SaleEndDate { get; set; }
}
