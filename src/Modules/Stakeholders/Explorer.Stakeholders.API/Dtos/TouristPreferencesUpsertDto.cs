namespace Explorer.Stakeholders.API.Dtos;

public class TouristPreferencesUpsertDto
{
    public int? PreferredDifficulty { get; set; }
    public int WalkRating { get; set; }
    public int BikeRating { get; set; }
    public int CarRating { get; set; }
    public int BoatRating { get; set; }
    public List<string> Tags { get; set; } = new();
}
