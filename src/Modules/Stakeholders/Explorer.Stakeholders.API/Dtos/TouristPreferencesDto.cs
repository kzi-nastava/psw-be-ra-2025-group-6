namespace Explorer.Stakeholders.API.Dtos;

public class TouristPreferencesDto
{
    public long TouristId { get; set; }
    public int? PreferredDifficulty { get; set; }
    public int WalkRating { get; set; }
    public int BikeRating { get; set; }
    public int CarRating { get; set; }
    public int BoatRating { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastSeenRecommendationsAt { get; set; }
    public DateTime? LastNotifiedAt { get; set; }
}
