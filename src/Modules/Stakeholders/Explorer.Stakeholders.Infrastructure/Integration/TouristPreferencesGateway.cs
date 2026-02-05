using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Tourist;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Integration;

public class TouristPreferencesGateway : ITouristPreferencesGateway
{
    private readonly ITouristPreferencesRepository _repository;

    public TouristPreferencesGateway(ITouristPreferencesRepository repository)
    {
        _repository = repository;
    }

    public TouristPreferencesSnapshot? GetByTouristId(long touristId)
    {
        var preferences = _repository.GetByTouristId(touristId);
        if (preferences == null) return null;

        return new TouristPreferencesSnapshot
        {
            PreferredDifficulty = preferences.PreferredDifficulty,
            WalkRating = preferences.WalkRating,
            BikeRating = preferences.BikeRating,
            CarRating = preferences.CarRating,
            BoatRating = preferences.BoatRating,
            Tags = NormalizeTags(preferences.Tags)
        };
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
