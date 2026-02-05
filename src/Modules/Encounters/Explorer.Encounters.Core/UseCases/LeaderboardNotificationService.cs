using Explorer.Encounters.Core.Domain;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Encounters.Core.UseCases;

public class LeaderboardNotificationService : ILeaderboardNotificationService
{
    private readonly IInternalNotificationService _notificationService;
    private const long SystemSenderId = -1; // System sender ID for automated notifications

    public LeaderboardNotificationService(IInternalNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void NotifyRankChange(long userId, int oldRank, int newRank)
    {
        if (oldRank == newRank) return;

        string content;
        string notificationType;

        if (newRank < oldRank)
        {
            var positionsGained = oldRank - newRank;
            content = positionsGained == 1
                ? $"Congratulations! You've moved up to rank #{newRank} on the leaderboard!"
                : $"You climbed {positionsGained} positions! You're now ranked #{newRank}.";
            notificationType = "RankIncreased";
        }
        else
        {
            var positionsLost = newRank - oldRank;
            content = positionsLost == 1
                ? $"Someone passed you! You dropped to rank #{newRank}."
                : $"You dropped {positionsLost} positions. You're now ranked #{newRank}.";
            notificationType = "RankDecreased";
        }

        _notificationService.CreateNotification(userId, SystemSenderId, content, userId, notificationType);
    }

    public void NotifyTop10Entry(long userId)
    {
        _notificationService.CreateNotification(
            userId,
            SystemSenderId,
            "Amazing! You've reached the TOP 10 on the leaderboard!",
            userId,
            "EnteredTop10"
        );
    }

    public void NotifyTop3Entry(long userId)
    {
        _notificationService.CreateNotification(
            userId,
            SystemSenderId,
            "Incredible! You've entered the TOP 3 on the leaderboard!",
            userId,
            "EnteredTop3"
        );
    }

    public void NotifyBecameFirst(long userId)
    {
        _notificationService.CreateNotification(
            userId,
            SystemSenderId,
            "Congratulations! You're now #1 on the leaderboard!",
            userId,
            "BecameFirst"
        );
    }

    public void NotifyMilestoneXP(long userId, int xpAmount)
    {
        string content = xpAmount switch
        {
            1000 => "Milestone reached: 1,000 XP earned! Keep exploring!",
            5000 => "Impressive! You've earned 5,000 XP!",
            10000 => "Outstanding! 10,000 XP milestone unlocked!",
            _ => $"Milestone reached: {xpAmount:N0} XP earned!"
        };

        _notificationService.CreateNotification(userId, SystemSenderId, content, userId, "MilestoneXP");
    }

    public void NotifyMilestoneChallenges(long userId, int challengeCount)
    {
        string content = challengeCount switch
        {
            10 => "You've completed 10 challenges! Great start!",
            50 => "Wow! 50 challenges completed! You're on fire!",
            100 => "Legendary! You've completed 100 challenges!",
            _ => $"Milestone: {challengeCount} challenges completed!"
        };

        _notificationService.CreateNotification(userId, SystemSenderId, content, userId, "MilestoneChallenges");
    }

    public void NotifyMilestoneTours(long userId, int tourCount)
    {
        string content = tourCount switch
        {
            10 => "You've completed 10 tours! Keep exploring!",
            50 => "Amazing! 50 tours completed! You're a true explorer!",
            100 => "Phenomenal! You've completed 100 tours!",
            _ => $"Milestone: {tourCount} tours completed!"
        };

        _notificationService.CreateNotification(userId, SystemSenderId, content, userId, "MilestoneTours");
    }

    public void NotifyClubRankChange(long clubId, string clubName, int oldRank, int newRank, List<long> memberIds)
    {
        if (oldRank == newRank) return;

        string content = newRank < oldRank
            ? $"Your club '{clubName}' moved up to rank #{newRank}!"
            : $"Your club '{clubName}' dropped to rank #{newRank}.";

        _notificationService.CreateNotificationsForMultipleRecipients(
            memberIds,
            SystemSenderId,
            content,
            clubId,
            "ClubRankChanged"
        );
    }

    public void NotifyNearRanking(long userId, int xpDifference, int targetRank)
    {
        _notificationService.CreateNotification(
            userId,
            SystemSenderId,
            $"You're only {xpDifference} XP away from rank #{targetRank}! Complete another challenge!",
            userId,
            "NearRankingAlert"
        );
    }
}
