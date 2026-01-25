using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Internal;

namespace Explorer.Encounters.Core.UseCases;

public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardEntryRepository _leaderboardEntryRepository;
    private readonly IClubLeaderboardRepository _clubLeaderboardRepository;
    private readonly ITouristXpProfileRepository _xpProfileRepository;
    private readonly IEncounterCompletionRepository _encounterCompletionRepository;
    private readonly IInternalStakeholderService _stakeholderService;
    private readonly IInternalTourService _tourService;
    private readonly IMapper _mapper;

    public LeaderboardService(
        ILeaderboardEntryRepository leaderboardEntryRepository,
        IClubLeaderboardRepository clubLeaderboardRepository,
        ITouristXpProfileRepository xpProfileRepository,
        IEncounterCompletionRepository encounterCompletionRepository,
        IInternalStakeholderService stakeholderService,
        IInternalTourService tourService,
        IMapper mapper)
    {
        _leaderboardEntryRepository = leaderboardEntryRepository;
        _clubLeaderboardRepository = clubLeaderboardRepository;
        _xpProfileRepository = xpProfileRepository;
        _encounterCompletionRepository = encounterCompletionRepository;
        _stakeholderService = stakeholderService;
        _tourService = tourService;
        _mapper = mapper;
    }

    public async Task<PagedResult<LeaderboardEntryDto>> GetTouristLeaderboardAsync(int page, int pageSize, string? filter = null)
    {
        DateTime? fromDate = ParseFilter(filter);
        var entries = await _leaderboardEntryRepository.GetTopEntriesAsync(pageSize * page, fromDate);
        
        // Paginate
        var pagedEntries = entries
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<List<LeaderboardEntryDto>>(pagedEntries);
        
        return new PagedResult<LeaderboardEntryDto>(dtos, entries.Count);
    }

    public async Task<PagedResult<ClubLeaderboardDto>> GetClubLeaderboardAsync(int page, int pageSize)
    {
        var clubs = await _clubLeaderboardRepository.GetTopClubsAsync(pageSize * page);
        
        // Paginate
        var pagedClubs = clubs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<List<ClubLeaderboardDto>>(pagedClubs);
        
        return new PagedResult<ClubLeaderboardDto>(dtos, clubs.Count);
    }

    public async Task<LeaderboardEntryDto> GetUserLeaderboardStatsAsync(long userId)
    {
        var entry = await _leaderboardEntryRepository.GetByUserIdAsync(userId);
        if (entry == null)
        {
            // Create new entry if doesn't exist
            entry = new LeaderboardEntry(userId, $"User_{userId}");
            entry = _leaderboardEntryRepository.Create(entry);
        }

        return _mapper.Map<LeaderboardEntryDto>(entry);
    }

    public async Task<ClubLeaderboardDto> GetClubLeaderboardStatsAsync(long clubId)
    {
        var clubLeaderboard = await _clubLeaderboardRepository.GetByClubIdAsync(clubId);
        if (clubLeaderboard == null)
        {
            throw new KeyNotFoundException($"Club leaderboard not found for club {clubId}");
        }

        return _mapper.Map<ClubLeaderboardDto>(clubLeaderboard);
    }

    public async Task UpdateUserStatsAsync(long userId, int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned)
    {
        var entry = await _leaderboardEntryRepository.GetByUserIdAsync(userId);
        if (entry == null)
        {
            entry = new LeaderboardEntry(userId, $"User_{userId}");
            entry.UpdateStats(xpGained, challengesCompleted, toursCompleted, coinsEarned);
            _leaderboardEntryRepository.Create(entry);
        }
        else
        {
            entry.UpdateStats(xpGained, challengesCompleted, toursCompleted, coinsEarned);
            _leaderboardEntryRepository.Update(entry);
        }

        // Update club stats if user is in a club
        if (entry.ClubId.HasValue)
        {
            await UpdateClubStatsAsync(entry.ClubId.Value, xpGained, challengesCompleted, toursCompleted, coinsEarned);
        }

        // Recalculate ranks
        await RecalculateRanksAsync();
    }

    public async Task UpdateClubStatsAsync(long clubId, int xpGained, int challengesCompleted, int toursCompleted, int coinsEarned)
    {
        var clubLeaderboard = await _clubLeaderboardRepository.GetByClubIdAsync(clubId);
        if (clubLeaderboard == null)
        {
            throw new KeyNotFoundException($"Club leaderboard not found for club {clubId}");
        }

        clubLeaderboard.UpdateStats(xpGained, challengesCompleted, toursCompleted, coinsEarned);
        _clubLeaderboardRepository.Update(clubLeaderboard);

        // Recalculate club ranks
        await RecalculateClubRanksAsync();
    }

    public async Task RecalculateRanksAsync()
    {
        var allEntries = await _leaderboardEntryRepository.GetTopEntriesAsync(int.MaxValue);
        var sortedEntries = allEntries
            .OrderByDescending(e => e.TotalXP)
            .ThenByDescending(e => e.CompletedChallenges)
            .ThenByDescending(e => e.CompletedTours)
            .ToList();

        for (int i = 0; i < sortedEntries.Count; i++)
        {
            sortedEntries[i].UpdateRank(i + 1);
            _leaderboardEntryRepository.Update(sortedEntries[i]);
        }

        await RecalculateClubRanksAsync();
    }

    private async Task RecalculateClubRanksAsync()
    {
        var allClubs = await _clubLeaderboardRepository.GetTopClubsAsync(int.MaxValue);
        var sortedClubs = allClubs
            .OrderByDescending(c => c.TotalXP)
            .ThenByDescending(c => c.TotalCompletedChallenges)
            .ThenByDescending(c => c.TotalCompletedTours)
            .ToList();

        for (int i = 0; i < sortedClubs.Count; i++)
        {
            sortedClubs[i].UpdateRank(i + 1);
            _clubLeaderboardRepository.Update(sortedClubs[i]);
        }
    }

    private DateTime? ParseFilter(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter)) return null;

        return filter.ToLower() switch
        {
            "day" => DateTime.UtcNow.AddDays(-1),
            "week" => DateTime.UtcNow.AddDays(-7),
            "month" => DateTime.UtcNow.AddMonths(-1),
            _ => null
        };
    }

    public async Task InitializeLeaderboardForAllUsersAsync()
    {
        var tourists = _stakeholderService.GetAllTouristsWithPersonIds();
        
        foreach (var (personId, username) in tourists)
        {
            var existingEntry = await _leaderboardEntryRepository.GetByUserIdAsync(personId);
            if (existingEntry == null)
            {
                // ? Calculate real stats from database using PersonId (TouristId)
                var xpProfile = _xpProfileRepository.GetByUserId(personId);
                var totalXP = xpProfile?.CurrentXP ?? 0;
                
                // Count completed challenges
                var completedEncounters = _encounterCompletionRepository.GetByUserId(personId);
                var completedChallenges = completedEncounters.Count;
                
                // ? Count completed tours from Tours module using PersonId (TouristId)
                var completedTours = _tourService.GetCompletedToursCountForUser(personId);
                
                // Calculate adventure coins (XP / 2)
                var adventureCoins = totalXP / 2;
                
                // Create entry with real data from database
                var newEntry = new LeaderboardEntry(personId, username);
                if (totalXP > 0 || completedChallenges > 0 || completedTours > 0)
                {
                    newEntry.UpdateStats(totalXP, completedChallenges, completedTours, adventureCoins);
                }
                
                _leaderboardEntryRepository.Create(newEntry);
            }
        }

        await RecalculateRanksAsync();
    }

    // ? NEW: Club management methods

    public async Task UserJoinedClubAsync(long userId, long clubId)
    {
        var entry = await _leaderboardEntryRepository.GetByUserIdAsync(userId);
        if (entry == null)
        {
            // Create entry if doesn't exist
            var username = _stakeholderService.GetUsername(userId);
            entry = new LeaderboardEntry(userId, username, clubId);
            _leaderboardEntryRepository.Create(entry);
        }
        else
        {
            entry.JoinClub(clubId);
            _leaderboardEntryRepository.Update(entry);
        }

        // Update club member count
        var clubLeaderboard = await _clubLeaderboardRepository.GetByClubIdAsync(clubId);
        if (clubLeaderboard != null)
        {
            var memberCount = await GetClubMemberCountAsync(clubId);
            clubLeaderboard.UpdateMemberCount(memberCount);
            _clubLeaderboardRepository.Update(clubLeaderboard);
        }
    }

    public async Task UserLeftClubAsync(long userId)
    {
        var entry = await _leaderboardEntryRepository.GetByUserIdAsync(userId);
        if (entry == null) return;

        var oldClubId = entry.ClubId;
        entry.LeaveClub();
        _leaderboardEntryRepository.Update(entry);

        // Update old club member count
        if (oldClubId.HasValue)
        {
            var clubLeaderboard = await _clubLeaderboardRepository.GetByClubIdAsync(oldClubId.Value);
            if (clubLeaderboard != null)
            {
                var memberCount = await GetClubMemberCountAsync(oldClubId.Value);
                clubLeaderboard.UpdateMemberCount(memberCount);
                _clubLeaderboardRepository.Update(clubLeaderboard);
            }
        }
    }

    public async Task InitializeClubLeaderboardAsync(long clubId, string clubName)
    {
        var existing = await _clubLeaderboardRepository.GetByClubIdAsync(clubId);
        if (existing != null) return; // Already exists

        var memberCount = await GetClubMemberCountAsync(clubId);
        var clubLeaderboard = new ClubLeaderboard(clubId, clubName, memberCount);
        
        // Calculate initial stats from all members
        var members = await _leaderboardEntryRepository.GetEntriesByClubIdAsync(clubId);
        var totalXP = members.Sum(m => m.TotalXP);
        var totalChallenges = members.Sum(m => m.CompletedChallenges);
        var totalTours = members.Sum(m => m.CompletedTours);
        var totalCoins = members.Sum(m => m.AdventureCoins);

        if (totalXP > 0 || totalChallenges > 0 || totalTours > 0)
        {
            clubLeaderboard.UpdateStats(totalXP, totalChallenges, totalTours, totalCoins);
        }

        _clubLeaderboardRepository.Create(clubLeaderboard);
        await RecalculateClubRanksAsync();
    }

    public async Task InitializeAllClubLeaderboardsAsync()
    {
        var clubs = _stakeholderService.GetAllClubs();
        
        foreach (var (clubId, clubName) in clubs)
        {
            await InitializeClubLeaderboardAsync(clubId, clubName);
        }
    }

    private async Task<int> GetClubMemberCountAsync(long clubId)
    {
        var members = await _leaderboardEntryRepository.GetEntriesByClubIdAsync(clubId);
        return members.Count;
    }

    public async Task<List<LeaderboardEntryDto>> GetClubMembersAsync(long clubId)
    {
        var members = await _leaderboardEntryRepository.GetEntriesByClubIdAsync(clubId);
        return _mapper.Map<List<LeaderboardEntryDto>>(members);
    }
}
