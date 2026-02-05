using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class TouristPreferencesDatabaseRepository(
    StakeholdersContext dbContext,
    ILogger<TouristPreferencesDatabaseRepository> logger)
    : CrudDatabaseRepository<TouristPreferences, StakeholdersContext>(dbContext), ITouristPreferencesRepository
{
    private readonly StakeholdersContext _dbContext = dbContext;
    private readonly ILogger<TouristPreferencesDatabaseRepository> _logger = logger;

    public TouristPreferences? GetByTouristId(long touristId)
    {
        var preferences = _dbContext.TouristPreferences.FirstOrDefault(p => p.TouristId == touristId);
        _logger.LogDebug("TouristPreferences lookup: touristId={TouristId} found={Found}", touristId, preferences != null);
        return preferences;
    }

    public List<TouristPreferences> GetAll()
    {
        return _dbContext.TouristPreferences.ToList();
    }
}
