using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;
public class AnnualAward : Entity
{
    public string Name { get; private set; }
    public string Description {  get; private set; }
    public int Year { get; private set; }
    public AwardStatus Status { get; private set; }
    public DateTime VotingStartDate { get; private set; }
    public DateTime VotingEndDate { get; private set; }

    public AnnualAward(string name, string description, int year, AwardStatus status, DateTime votingStartDate, DateTime votingEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid name.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Invalid description.");

        if (year < 2000 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year.");

        if (!Enum.IsDefined(typeof(AwardStatus), status))
            throw new ArgumentException("Invalid award status");

        if (votingStartDate >= votingEndDate)
            throw new ArgumentException("Voting start date must be before voting end date.");

        Name = name;
        Description = description;
        Year = year;
        Status = AwardStatus.DRAFT;
        VotingStartDate = votingStartDate;
        VotingEndDate = votingEndDate;
    }
}
