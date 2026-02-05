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
        // Detach any tracked entities to force fresh read from database
        var trackedEntity = _context.ChangeTracker.Entries<ClubLeaderboard>()
            .FirstOrDefault(e => e.Entity.ClubId == clubId);
        
        if (trackedEntity != null)
        {
            // Reload from database to get fresh data
            await trackedEntity.ReloadAsync();
            return trackedEntity.Entity;
        }

        return await _context.ClubLeaderboards
            .FirstOrDefaultAsync(c => c.ClubId == clubId);
    }

    public async Task<List<ClubLeaderboard>> GetTopClubsAsync(int count)
    {
        // Clear all tracked entities to ensure fresh read from database
        _context.ChangeTracker.Clear();
        
        return await _context.ClubLeaderboards
            .AsNoTracking()
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
            .AsNoTracking()
            .Where(c => c.TotalXP > club.TotalXP || 
                       (c.TotalXP == club.TotalXP && c.TotalCompletedChallenges > club.TotalCompletedChallenges) ||
                       (c.TotalXP == club.TotalXP && c.TotalCompletedChallenges == club.TotalCompletedChallenges && c.TotalCompletedTours > club.TotalCompletedTours))
            .CountAsync();

        return rank + 1;
    }

    public new ClubLeaderboard Update(ClubLeaderboard club)
    {
        // Detach any existing tracked entity with same ID to avoid conflicts
        var existingEntry = _context.ChangeTracker.Entries<ClubLeaderboard>()
            .FirstOrDefault(e => e.Entity.Id == club.Id);
        
        if (existingEntry != null && existingEntry.Entity != club)
        {
            existingEntry.State = EntityState.Detached;
        }

        // Explicitly mark entry as modified
        _context.Entry(club).State = EntityState.Modified;
        _context.SaveChanges();
        
        // Clear change tracker after save to prevent stale data
        _context.ChangeTracker.Clear();
        
        return club;
    }
}
