using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.Core.UseCases;

public interface ILeaderboardService
{
    Task<PagedResult<LeaderboardEntryDto>> GetTouristLeaderboardAsync(int page, int pageSize, string? filter = null);
    Task<PagedResult<ClubLeaderboardDto>> GetClubLeaderboardAsync(int page, int pageSize);
    Task<LeaderboardEntryDto> GetUserLeaderboardStatsAsync(long userId);
    Task<ClubLeaderboardDto> GetClubLeaderboardStatsAsync(long clubId);
    Task UpdateUserStatsAsync(long userId, int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned);
    Task UpdateClubStatsAsync(long clubId, int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned);
    Task RecalculateRanksAsync();
    Task InitializeLeaderboardForAllUsersAsync();
    
    // ? NEW: Club management methods
    Task UserJoinedClubAsync(long userId, long clubId);
    Task UserLeftClubAsync(long userId);
    Task InitializeClubLeaderboardAsync(long clubId, string clubName);
    Task InitializeAllClubLeaderboardsAsync();
    Task<List<LeaderboardEntryDto>> GetClubMembersAsync(long clubId);
}
