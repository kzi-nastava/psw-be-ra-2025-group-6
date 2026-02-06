namespace Explorer.Encounters.API.Dtos;

public class LeaderboardEntryDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Username { get; set; }
    public int TotalXP { get; set; }
    public int CompletedChallenges { get; set; }
    public int CompletedTours { get; set; }
    public int AdventureCoins { get; set; }
    public int CurrentRank { get; set; }
    public DateTime LastUpdated { get; set; }
    public long? ClubId { get; set; }
}

public class ClubLeaderboardDto
{
    public long Id { get; set; }
    public long ClubId { get; set; }
    public string ClubName { get; set; }
    public int TotalXP { get; set; }
    public int TotalCompletedChallenges { get; set; }
    public int TotalCompletedTours { get; set; }
    public int TotalAdventureCoins { get; set; }
    public int MemberCount { get; set; }
    public int CurrentRank { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class UpdateLeaderboardStatsDto
{
    public long UserId { get; set; }
    public int XpGained { get; set; }
    public int ChallengesCompleted { get; set; }
    public int ToursCompleted { get; set; }
    public int CoinsEarned { get; set; }
}
