namespace Explorer.Tours.API.Dtos;

public class MeetupDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long CreatorId { get; set; }
    public DateTime LastModified { get; set; }
}
