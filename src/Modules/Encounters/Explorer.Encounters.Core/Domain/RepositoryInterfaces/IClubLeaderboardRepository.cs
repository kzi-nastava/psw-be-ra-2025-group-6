using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces;

public interface IClubLeaderboardRepository : ICrudRepository<ClubLeaderboard>
{
    Task<ClubLeaderboard?> GetByClubIdAsync(long clubId);
    Task<List<ClubLeaderboard>> GetTopClubsAsync(int count);
    Task<int> GetClubRankAsync(long clubId);
}
