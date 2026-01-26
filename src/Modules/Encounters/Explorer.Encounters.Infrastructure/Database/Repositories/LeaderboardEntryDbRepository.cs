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
        // Detach any tracked entities to force fresh read from database
        var trackedEntity = _context.ChangeTracker.Entries<LeaderboardEntry>()
            .FirstOrDefault(e => e.Entity.UserId == userId);
        
        if (trackedEntity != null)
        {
            // Reload from database to get fresh data
            await trackedEntity.ReloadAsync();
            return trackedEntity.Entity;
        }

        return await _context.LeaderboardEntries
            .FirstOrDefaultAsync(e => e.UserId == userId);
    }

    public async Task<List<LeaderboardEntry>> GetTopEntriesAsync(int count, DateTime? fromDate = null)
    {
        // Clear all tracked entities to ensure fresh read from database
        _context.ChangeTracker.Clear();
        
        var query = _context.LeaderboardEntries.AsNoTracking().AsQueryable();

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
        // Clear tracked entities for fresh read
        _context.ChangeTracker.Clear();
        
        return await _context.LeaderboardEntries
            .AsNoTracking()
            .Where(e => e.ClubId == clubId)
            .OrderByDescending(e => e.TotalXP)
            .ToListAsync();
    }

    public async Task<int> GetUserRankAsync(long userId)
    {
        var entry = await GetByUserIdAsync(userId);
        if (entry == null) return 0;

        var rank = await _context.LeaderboardEntries
            .AsNoTracking()
            .Where(e => e.TotalXP > entry.TotalXP || 
                       (e.TotalXP == entry.TotalXP && e.CompletedChallenges > entry.CompletedChallenges) ||
                       (e.TotalXP == entry.TotalXP && e.CompletedChallenges == entry.CompletedChallenges && e.CompletedTours > entry.CompletedTours))
            .CountAsync();

        return rank + 1;
    }

    public new LeaderboardEntry Update(LeaderboardEntry entry)
    {
        // Detach any existing tracked entity with same ID to avoid conflicts
        var existingEntry = _context.ChangeTracker.Entries<LeaderboardEntry>()
            .FirstOrDefault(e => e.Entity.Id == entry.Id);
        
        if (existingEntry != null && existingEntry.Entity != entry)
        {
            existingEntry.State = EntityState.Detached;
        }

        // Explicitly mark entry as modified
        _context.Entry(entry).State = EntityState.Modified;
        _context.SaveChanges();
        
        // Clear change tracker after save to prevent stale data
        _context.ChangeTracker.Clear();
        
        return entry;
    }
}
