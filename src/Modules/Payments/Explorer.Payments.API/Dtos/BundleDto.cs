namespace Explorer.Payments.API.Dtos;

public class BundleDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Status { get; set; }
    public List<long> TourIds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class CreateBundleDto
{
    public string Name { get; set; }
    public double Price { get; set; }
    public List<long> TourIds { get; set; }
}

public class UpdateBundleDto
{
    public string Name { get; set; }
    public double Price { get; set; }
    public List<long> TourIds { get; set; }
}
