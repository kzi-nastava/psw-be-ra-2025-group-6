using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface ITouristPreferencesService
{
    TouristPreferencesDto? GetByTouristId(long touristId);
    TouristPreferencesDto Upsert(long touristId, TouristPreferencesUpsertDto dto);
    void Delete(long touristId);
}
