namespace Explorer.Encounters.Core.UseCases;

public interface ILeaderboardNotificationService
{
    void NotifyRankChange(long userId, int oldRank, int newRank);
    void NotifyTop10Entry(long userId);
    void NotifyTop3Entry(long userId);
    void NotifyBecameFirst(long userId);
    void NotifyMilestoneXP(long userId, int xpAmount);
    void NotifyMilestoneChallenges(long userId, int challengeCount);
    void NotifyMilestoneTours(long userId, int tourCount);
    void NotifyClubRankChange(long clubId, string clubName, int oldRank, int newRank, List<long> memberIds);
    void NotifyNearRanking(long userId, int xpDifference, int targetRank);
}
