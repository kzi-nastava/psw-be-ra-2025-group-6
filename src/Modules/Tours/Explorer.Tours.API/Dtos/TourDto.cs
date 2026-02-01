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
    public string? CoverImage { get; set; }

    public List<EquipmentDto>? Equipment { get; set; }
    public List<KeyPointDto>? KeyPoints { get; set; } = new List<KeyPointDto>();
    public double DistanceInKm { get; set; }
    public double? MarkerLat { get; set; }
    public double? MarkerLng { get; set; }
    public List<TourDurationDto>? Duration { get; set;  }
    public DateTime? PublishedTime { get; set; }
}
