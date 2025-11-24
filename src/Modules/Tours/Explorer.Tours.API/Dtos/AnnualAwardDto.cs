namespace Explorer.Tours.API.Dtos;
public class AnnualAwardDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Year { get; set; }
    public AwardStatusDto Status { get; set; }
    public DateTime VotingStartDate { get; set; }
    public DateTime VotingEndDate { get; set; }
}
