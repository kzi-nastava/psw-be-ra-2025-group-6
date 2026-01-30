using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories;

public class ClubLeaderboardDbRepository : CrudDatabaseRepository<ClubLeaderboard, EncountersContext>, IClubLeaderboardRepository
{
    private readonly EncountersContext _context;

    public ClubLeaderboardDbRepository(EncountersContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ClubLeaderboard?> GetByClubIdAsync(long clubId)
    {
        return await _context.ClubLeaderboards
            .FirstOrDefaultAsync(c => c.ClubId == clubId);
    }

    public async Task<List<ClubLeaderboard>> GetTopClubsAsync(int count)
    {
        return await _context.ClubLeaderboards
            .OrderByDescending(c => c.TotalXP)
            .ThenByDescending(c => c.TotalCompletedChallenges)
            .ThenByDescending(c => c.TotalCompletedTours)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetClubRankAsync(long clubId)
    {
        var club = await GetByClubIdAsync(clubId);
        if (club == null) return 0;

        var rank = await _context.ClubLeaderboards
            .Where(c => c.TotalXP > club.TotalXP || 
                       (c.TotalXP == club.TotalXP && c.TotalCompletedChallenges > club.TotalCompletedChallenges) ||
                       (c.TotalXP == club.TotalXP && c.TotalCompletedChallenges == club.TotalCompletedChallenges && c.TotalCompletedTours > club.TotalCompletedTours))
            .CountAsync();

        return rank + 1;
    }
}
