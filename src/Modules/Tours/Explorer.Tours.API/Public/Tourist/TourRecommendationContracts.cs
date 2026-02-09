using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITouristPreferencesGateway
{
    TouristPreferencesSnapshot? GetByTouristId(long touristId);
}

public class TouristPreferencesSnapshot
{
    public int? PreferredDifficulty { get; init; }
    public int WalkRating { get; init; }
    public int BikeRating { get; init; }
    public int CarRating { get; init; }
    public int BoatRating { get; init; }
    public List<string> Tags { get; init; } = new();
    public DateTime? LastSeenRecommendationsAt { get; init; }
    public DateTime? LastNotifiedAt { get; init; }
}

public class TourRecommendationsDto
{
    public List<long> TourIds { get; init; } = new();
    public List<long> NewTourIds { get; init; } = new();
}

public class TourRecommendationSummary
{
    public List<long> TourIds { get; init; } = new();
    public List<long> NewTourIds { get; init; } = new();
    public int NewMatchingCount { get; init; }
    public DateTime? NewestMatchingPublishedAt { get; init; }
    public long? NewestMatchingTourId { get; init; }
    public long? NewestMatchingTourAuthorId { get; init; }
}

public static class TourRecommendationScoring
{
    public const int RecommendationThreshold = 3;
    private const int DifficultyScore = 3;
    private const int TagScore = 2;
    private const int MaxTagScore = 6;
    private const int TransportScore = 1;

    public static int CalculateScore(TourDto tour, TouristPreferencesSnapshot preferences)
    {
        var score = 0;

        if (IsDifficultyMatch(tour, preferences)) score += DifficultyScore;

        var tagScore = CalculateTagScore(tour.Tags, preferences.Tags);
        score += tagScore;

        if (HasTransportMatch(tour, preferences)) score += TransportScore;

        return score;
    }

    private static int CalculateTagScore(List<string>? tourTags, List<string> preferenceTags)
    {
        if (tourTags == null || tourTags.Count == 0 || preferenceTags.Count == 0) return 0;

        var normalizedTourTags = tourTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var normalizedPreferences = preferenceTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant());
        var matches = normalizedPreferences.Count(tag => normalizedTourTags.Contains(tag));
        var score = matches * TagScore;
        return Math.Min(score, MaxTagScore);
    }

    public static int GetMatchedTagsCount(List<string>? tourTags, List<string> preferenceTags)
    {
        if (tourTags == null || tourTags.Count == 0 || preferenceTags.Count == 0) return 0;

        var normalizedTourTags = tourTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var normalizedPreferences = preferenceTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant());
        return normalizedPreferences.Count(tag => normalizedTourTags.Contains(tag));
    }

    public static bool IsDifficultyMatch(TourDto tour, TouristPreferencesSnapshot preferences)
    {
        return preferences.PreferredDifficulty.HasValue &&
            preferences.PreferredDifficulty.Value == (int)tour.Difficulty;
    }

    public static bool HasTransportMatchForLogging(TourDto tour, TouristPreferencesSnapshot preferences)
    {
        return HasTransportMatch(tour, preferences);
    }

    private static bool HasTransportMatch(TourDto tour, TouristPreferencesSnapshot preferences)
    {
        if (tour.Duration == null || tour.Duration.Count == 0) return false;

        foreach (var duration in tour.Duration)
        {
            if (GetRatingForTravelType(preferences, duration.TravelType) >= 2)
            {
                return true;
            }
        }

        return false;
    }

    private static int GetRatingForTravelType(TouristPreferencesSnapshot preferences, TravelTypeDto travelType)
    {
        return travelType switch
        {
            TravelTypeDto.WALK => preferences.WalkRating,
            TravelTypeDto.BIKE => preferences.BikeRating,
            TravelTypeDto.CAR => preferences.CarRating,
            TravelTypeDto.BOAT => preferences.BoatRating,
            _ => 0
        };
    }
}
