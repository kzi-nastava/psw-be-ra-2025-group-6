using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class TouristPreferencesService : ITouristPreferencesService
{
    private readonly ITouristPreferencesRepository _repository;
    private readonly IMapper _mapper;

    public TouristPreferencesService(ITouristPreferencesRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public TouristPreferencesDto? GetByTouristId(long touristId)
    {
        var preferences = _repository.GetByTouristId(touristId);
        if (preferences == null) return null;
        return _mapper.Map<TouristPreferencesDto>(preferences);
    }

    public TouristPreferencesDto Upsert(long touristId, TouristPreferencesUpsertDto dto)
    {
        var tags = NormalizeTags(dto.Tags);
        var existing = _repository.GetByTouristId(touristId);
        if (existing == null)
        {
            var created = new TouristPreferences(
                touristId,
                dto.PreferredDifficulty,
                dto.WalkRating,
                dto.BikeRating,
                dto.CarRating,
                dto.BoatRating,
                tags);
            var result = _repository.Create(created);
            return _mapper.Map<TouristPreferencesDto>(result);
        }

        existing.Update(
            dto.PreferredDifficulty,
            dto.WalkRating,
            dto.BikeRating,
            dto.CarRating,
            dto.BoatRating,
            tags);
        var updated = _repository.Update(existing);
        return _mapper.Map<TouristPreferencesDto>(updated);
    }

    public void Delete(long touristId)
    {
        var existing = _repository.GetByTouristId(touristId);
        if (existing == null) throw new NotFoundException("Tourist preferences not found.");
        _repository.Delete(existing.Id);
    }

    private static List<string> NormalizeTags(IEnumerable<string>? tags)
    {
        if (tags == null) return new List<string>();
        return tags
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
