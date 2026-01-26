using Explorer.Encounters.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Encounters.Core.UseCases;

public class LeaderboardNotificationService : ILeaderboardNotificationService
{
    private readonly INotificationService _notificationService;
    private const long SystemSenderId = -1; // System sender ID for automated notifications

    public LeaderboardNotificationService(INotificationService notificationService)
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

        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = content,
            ReferenceId = userId,
            Type = notificationType
        });
    }

    public void NotifyTop10Entry(long userId)
    {
        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = "Amazing! You've reached the TOP 10 on the leaderboard!",
            ReferenceId = userId,
            Type = "EnteredTop10"
        });
    }

    public void NotifyTop3Entry(long userId)
    {
        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = "Incredible! You've entered the TOP 3 on the leaderboard!",
            ReferenceId = userId,
            Type = "EnteredTop3"
        });
    }

    public void NotifyBecameFirst(long userId)
    {
        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = "Congratulations! You're now #1 on the leaderboard!",
            ReferenceId = userId,
            Type = "BecameFirst"
        });
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

        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = content,
            ReferenceId = userId,
            Type = "MilestoneXP"
        });
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

        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = content,
            ReferenceId = userId,
            Type = "MilestoneChallenges"
        });
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

        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = content,
            ReferenceId = userId,
            Type = "MilestoneTours"
        });
    }

    public void NotifyClubRankChange(long clubId, string clubName, int oldRank, int newRank, List<long> memberIds)
    {
        if (oldRank == newRank) return;

        string content = newRank < oldRank
            ? $"Your club '{clubName}' moved up to rank #{newRank}!"
            : $"Your club '{clubName}' dropped to rank #{newRank}.";

        foreach (var memberId in memberIds)
        {
            _notificationService.Create(new NotificationDto
            {
                RecipientId = memberId,
                SenderId = SystemSenderId,
                Content = content,
                ReferenceId = clubId,
                Type = "ClubRankChanged"
            });
        }
    }

    public void NotifyNearRanking(long userId, int xpDifference, int targetRank)
    {
        _notificationService.Create(new NotificationDto
        {
            RecipientId = userId,
            SenderId = SystemSenderId,
            Content = $"You're only {xpDifference} XP away from rank #{targetRank}! Complete another challenge!",
            ReferenceId = userId,
            Type = "NearRankingAlert"
        });
    }
}
