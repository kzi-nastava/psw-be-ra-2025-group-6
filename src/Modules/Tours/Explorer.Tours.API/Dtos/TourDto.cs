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
}
