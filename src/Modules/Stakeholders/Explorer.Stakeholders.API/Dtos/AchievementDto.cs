namespace Explorer.Stakeholders.API.Dtos;

public class AchievementDto
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string Category { get; set; }
    public int? Threshold { get; set; }
}

