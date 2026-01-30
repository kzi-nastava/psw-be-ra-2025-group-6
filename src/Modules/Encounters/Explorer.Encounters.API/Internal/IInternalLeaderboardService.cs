namespace Explorer.Encounters.API.Internal;

public interface IInternalLeaderboardService
{
    Task UpdateUserStatsAsync(long userId, int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned);
    void CreateLeaderboardEntryForNewUser(long userId, string username);
}
