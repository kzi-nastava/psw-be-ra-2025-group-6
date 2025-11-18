using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;
public class AnnualAward : Entity
{
    public string Name { get; init; }
    public string? Description {  get; init; }
    public int Year { get; init; }
    public AwardStatus Status { get; init; }
    public DateTime VotingStartDate { get; init; }
    public DateTime VotingEndDate { get; init; }

    
    public AnnualAward(string name, string? description, int year, AwardStatus status, DateTime votingStartDate, DateTime votingEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid name.");

        if (year < 2000 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year.");

        if (!Enum.IsDefined(typeof(AwardStatus), status))
            throw new ArgumentException("Invalid award status");

        if (votingStartDate >= votingEndDate)
            throw new ArgumentException("Voting start date must be before voting end date.");

        Name = name;
        Description = description;
        Year = year;
        Status = status;
        VotingStartDate = votingStartDate;
        VotingEndDate = votingEndDate;
    }
}
