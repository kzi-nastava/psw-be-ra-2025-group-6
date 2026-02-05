using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourRecommendationService(
    ITourRepository tourRepository,
    ITouristPreferencesGateway preferencesGateway,
    IMapper mapper,
    ILogger<TourRecommendationService> logger) : ITourRecommendationService
{
    private readonly ITourRepository _tourRepository = tourRepository;
    private readonly ITouristPreferencesGateway _preferencesGateway = preferencesGateway;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<TourRecommendationService> _logger = logger;

    public List<TourDto> GetRecommended(long touristId, int limit)
    {
        var preferences = _preferencesGateway.GetByTouristId(touristId);
        _logger.LogDebug("TourRecommendations: touristId={TouristId} preferencesFound={Found}", touristId, preferences != null);
        if (preferences == null) return new List<TourDto>();

        var tours = _tourRepository.GetPublishedTours();
        _logger.LogDebug("TourRecommendations: candidateCount={CandidateCount} filter=Status.CONFIRMED && PublishedTime != null", tours.Count);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            foreach (var tour in tours.Take(10))
            {
                var score = TourRecommendationScoring.CalculateScore(tour, preferences);
                var matchedTagsCount = TourRecommendationScoring.GetMatchedTagsCount(tour.Tags, preferences.Tags);
                var difficultyMatch = TourRecommendationScoring.IsDifficultyMatch(tour, preferences);
                var travelMatch = TourRecommendationScoring.HasTransportMatchForLogging(tour, preferences);
                var tags = tour.Tags == null ? string.Empty : string.Join(',', tour.Tags);
                _logger.LogDebug(
                    "TourRecommendations candidate: tourId={TourId} tags={Tags} difficulty={Difficulty} publishedTime={PublishedTime} matchedTags={MatchedTags} difficultyMatch={DifficultyMatch} travelMatch={TravelMatch} score={Score}",
                    tour.Id, tags, tour.Difficulty, tour.PublishedTime, matchedTagsCount, difficultyMatch, travelMatch, score);
            }
        }
        var scored = tours
            .Select(t => new
            {
                Tour = t,
                Score = TourRecommendationScoring.CalculateScore(t, preferences)
            })
            .Where(x => x.Score >= TourRecommendationScoring.RecommendationThreshold)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Tour.PublishedTime ?? DateTime.MinValue)
            .Take(limit)
            .Select(x => _mapper.Map<TourDto>(x.Tour))
            .ToList();

        return scored;
    }
}

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
}

public static class TourRecommendationScoring
{
    public const int RecommendationThreshold = 3;
    private const int DifficultyScore = 3;
    private const int TagScore = 2;
    private const int MaxTagScore = 6;
    private const int TransportScore = 1;

    public static int CalculateScore(Tour tour, TouristPreferencesSnapshot preferences)
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

    public static bool IsDifficultyMatch(Tour tour, TouristPreferencesSnapshot preferences)
    {
        return preferences.PreferredDifficulty.HasValue &&
            preferences.PreferredDifficulty.Value == (int)tour.Difficulty;
    }

    public static bool HasTransportMatchForLogging(Tour tour, TouristPreferencesSnapshot preferences)
    {
        return HasTransportMatch(tour, preferences);
    }

    private static bool HasTransportMatch(Tour tour, TouristPreferencesSnapshot preferences)
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

    private static int GetRatingForTravelType(TouristPreferencesSnapshot preferences, TravelType travelType)
    {
        return travelType switch
        {
            TravelType.WALK => preferences.WalkRating,
            TravelType.BIKE => preferences.BikeRating,
            TravelType.CAR => preferences.CarRating,
            TravelType.BOAT => preferences.BoatRating,
            _ => 0
        };
    }
}
