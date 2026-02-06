using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class LeaderboardEntry : Entity
{
    public long UserId { get; init; }
    public string Username { get; init; }
    public int TotalXP { get; private set; }
    public int CompletedChallenges { get; private set; }
    public int CompletedTours { get; private set; }
    public int AdventureCoins { get; private set; }
    public int CurrentRank { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public long? ClubId { get; private set; }
    
    public LeaderboardEntry(long userId, string username, long? clubId = null)
    {
        UserId = userId;
        Username = username;
        ClubId = clubId;
        TotalXP = 0;
        CompletedChallenges = 0;
        CompletedTours = 0;
        AdventureCoins = 0;
        CurrentRank = 0;
        LastUpdated = DateTime.UtcNow;
        
        Validate();
    }

    public void UpdateStats(int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned)
    {
        TotalXP += xpGained;
        CompletedChallenges += challengesCompleted;
        CompletedTours += toursCompleted;
        AdventureCoins += coinsEarned;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateRank(int newRank)
    {
        CurrentRank = newRank;
        LastUpdated = DateTime.UtcNow;
    }

    public void JoinClub(long clubId)
    {
        ClubId = clubId;
        LastUpdated = DateTime.UtcNow;
    }

    public void LeaveClub()
    {
        ClubId = null;
        LastUpdated = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid UserId");
        if (string.IsNullOrWhiteSpace(Username)) throw new ArgumentException("Username cannot be empty");
    }
}
