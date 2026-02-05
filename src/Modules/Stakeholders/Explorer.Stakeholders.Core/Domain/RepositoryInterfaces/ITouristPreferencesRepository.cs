using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface ITouristPreferencesRepository : ICrudRepository<TouristPreferences>
{
    TouristPreferences? GetByTouristId(long touristId);
    List<TouristPreferences> GetAll();
}
