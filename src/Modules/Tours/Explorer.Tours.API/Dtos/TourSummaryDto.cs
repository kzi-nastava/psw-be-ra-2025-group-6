namespace Explorer.Tours.API.Dtos;

public class TourSummaryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TourDifficultyDto Difficulty { get; set; }
    public List<string>? Tags { get; set; }
    public float Price { get; set; }
    public double? FirstKeyPointLatitude { get; set; }
    public double? FirstKeyPointLongitude { get; set; }
}
