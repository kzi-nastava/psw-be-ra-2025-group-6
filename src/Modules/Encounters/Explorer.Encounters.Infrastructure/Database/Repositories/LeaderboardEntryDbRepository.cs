using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories;

public class LeaderboardEntryDbRepository : CrudDatabaseRepository<LeaderboardEntry, EncountersContext>, ILeaderboardEntryRepository
{
    private readonly EncountersContext _context;

    public LeaderboardEntryDbRepository(EncountersContext context) : base(context)
    {
        _context = context;
    }

    public async Task<LeaderboardEntry?> GetByUserIdAsync(long userId)
    {
        return await _context.LeaderboardEntries
            .FirstOrDefaultAsync(e => e.UserId == userId);
    }

    public async Task<List<LeaderboardEntry>> GetTopEntriesAsync(int count, DateTime? fromDate = null)
    {
        var query = _context.LeaderboardEntries.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(e => e.LastUpdated >= fromDate.Value);
        }

        return await query
            .OrderByDescending(e => e.TotalXP)
            .ThenByDescending(e => e.CompletedChallenges)
            .ThenByDescending(e => e.CompletedTours)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<LeaderboardEntry>> GetEntriesByClubIdAsync(long clubId)
    {
        return await _context.LeaderboardEntries
            .Where(e => e.ClubId == clubId)
            .OrderByDescending(e => e.TotalXP)
            .ToListAsync();
    }

    public async Task<int> GetUserRankAsync(long userId)
    {
        var entry = await GetByUserIdAsync(userId);
        if (entry == null) return 0;

        var rank = await _context.LeaderboardEntries
            .Where(e => e.TotalXP > entry.TotalXP || 
                       (e.TotalXP == entry.TotalXP && e.CompletedChallenges > entry.CompletedChallenges) ||
                       (e.TotalXP == entry.TotalXP && e.CompletedChallenges == entry.CompletedChallenges && e.CompletedTours > entry.CompletedTours))
            .CountAsync();

        return rank + 1;
    }
}
