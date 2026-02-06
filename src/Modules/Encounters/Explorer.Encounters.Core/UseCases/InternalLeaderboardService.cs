using Explorer.Encounters.API.Internal;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases;

public class InternalLeaderboardService : IInternalLeaderboardService
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ILeaderboardEntryRepository _leaderboardEntryRepository;

    public InternalLeaderboardService(
        ILeaderboardService leaderboardService,
        ILeaderboardEntryRepository leaderboardEntryRepository)
    {
        _leaderboardService = leaderboardService;
        _leaderboardEntryRepository = leaderboardEntryRepository;
    }

    public async Task UpdateUserStatsAsync(long userId, int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned)
    {
        await _leaderboardService.UpdateUserStatsAsync(userId, xpGained, challengesCompleted, toursCompleted, coinsEarned);
    }

    public void CreateLeaderboardEntryForNewUser(long userId, string username)
    {
        try
        {
            // Check if entry already exists
            var existing = _leaderboardEntryRepository.GetByUserIdAsync(userId).Result;
            if (existing != null) return;

            // Create new entry with 0 values (will be updated when user gains XP)
            var newEntry = new LeaderboardEntry(userId, username);
            _leaderboardEntryRepository.Create(newEntry);
        }
        catch
        {
            // Silently fail - not critical for registration
        }
    }
}
