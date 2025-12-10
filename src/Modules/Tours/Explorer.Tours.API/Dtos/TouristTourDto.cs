using Explorer.Tours.API.Dtos;

public class TouristTourDto
{
    public string Name { get; set; }
    public KeyPointDto FirstKeyPoint { get; set; }
    public TourDifficultyDto Difficulty { get; set; }
    public double Price { get; set; }
    public List<string> Tags { get; set; }
    public double DistanceInKm { get; set; }
    public List<TourDurationDto> Duration { get; set; }
    public string Description { get; set; }
}
