using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces;

public interface ILeaderboardEntryRepository : ICrudRepository<LeaderboardEntry>
{
    Task<LeaderboardEntry?> GetByUserIdAsync(long userId);
    Task<List<LeaderboardEntry>> GetTopEntriesAsync(int count, DateTime? fromDate = null);
    Task<List<LeaderboardEntry>> GetEntriesByClubIdAsync(long clubId);
    Task<int> GetUserRankAsync(long userId);
}
