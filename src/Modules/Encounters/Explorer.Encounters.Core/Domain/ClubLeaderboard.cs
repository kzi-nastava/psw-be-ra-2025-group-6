using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class ClubLeaderboard : Entity
{
    public long ClubId { get; init; }
    public string ClubName { get; init; }
    public int TotalXP { get; private set; }
    public int TotalCompletedChallenges { get; private set; }
    public int TotalCompletedTours { get; private set; }
    public int TotalAdventureCoins { get; private set; }
    public int MemberCount { get; private set; }
    public int CurrentRank { get; private set; }
    public DateTime LastUpdated { get; private set; }
    
    public ClubLeaderboard(long clubId, string clubName, int memberCount = 0)
    {
        ClubId = clubId;
        ClubName = clubName;
        MemberCount = memberCount;
        TotalXP = 0;
        TotalCompletedChallenges = 0;
        TotalCompletedTours = 0;
        TotalAdventureCoins = 0;
        CurrentRank = 0;
        LastUpdated = DateTime.UtcNow;
        
        Validate();
    }

    public void UpdateStats(int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned)
    {
        TotalXP += xpGained;
        TotalCompletedChallenges += challengesCompleted;
        TotalCompletedTours += toursCompleted;
        TotalAdventureCoins += coinsEarned;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateRank(int newRank)
    {
        CurrentRank = newRank;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateMemberCount(int count)
    {
        MemberCount = count;
        LastUpdated = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (ClubId == 0) throw new ArgumentException("Invalid ClubId");
        if (string.IsNullOrWhiteSpace(ClubName)) throw new ArgumentException("ClubName cannot be empty");
    }
}
