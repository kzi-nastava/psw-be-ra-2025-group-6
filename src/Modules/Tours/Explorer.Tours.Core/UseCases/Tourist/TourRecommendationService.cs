using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Explorer.Tours.API.Public.Tourist;

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
        var recommended = GetRecommendedTours(preferences);
        if (limit <= 0) limit = 6;
        return recommended.Take(limit).ToList();
    }

    public TourRecommendationSummary GetRecommendationSummary(long touristId, int? limit = null)
    {
        var preferences = _preferencesGateway.GetByTouristId(touristId);
        var recommended = GetRecommendedTours(preferences);
        if (recommended.Count == 0)
        {
            return new TourRecommendationSummary();
        }

        var lastSeen = preferences?.LastSeenRecommendationsAt ?? DateTime.MinValue;
        var newTours = recommended
            .Where(t => (t.PublishedTime ?? DateTime.MinValue) > lastSeen)
            .ToList();

        var newestNewTour = newTours
            .OrderByDescending(t => t.PublishedTime ?? DateTime.MinValue)
            .FirstOrDefault();

        var limited = limit.HasValue ? recommended.Take(limit.Value).ToList() : recommended;
        var limitedIds = limited.Select(t => t.Id).ToList();
        var newLimitedIds = limited
            .Where(t => (t.PublishedTime ?? DateTime.MinValue) > lastSeen)
            .Select(t => t.Id)
            .ToList();

        return new TourRecommendationSummary
        {
            TourIds = limitedIds,
            NewTourIds = newLimitedIds,
            NewMatchingCount = newTours.Count,
            NewestMatchingPublishedAt = newestNewTour?.PublishedTime,
            NewestMatchingTourId = newestNewTour?.Id,
            NewestMatchingTourAuthorId = newestNewTour?.AuthorId
        };
    }

    private List<TourDto> GetRecommendedTours(TouristPreferencesSnapshot? preferences)
    {
        _logger.LogDebug("TourRecommendations: preferencesFound={Found}", preferences != null);
        if (preferences == null) return new List<TourDto>();

        var tours = _tourRepository.GetPublishedTours();
        _logger.LogDebug("TourRecommendations: candidateCount={CandidateCount} filter=Status.CONFIRMED && PublishedTime != null", tours.Count);
        var tourDtos = _mapper.Map<List<TourDto>>(tours);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            foreach (var tour in tourDtos.Take(10))
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

        return tourDtos
            .Select(t => new
            {
                Tour = t,
                Score = TourRecommendationScoring.CalculateScore(t, preferences)
            })
            .Where(x => x.Score >= TourRecommendationScoring.RecommendationThreshold)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Tour.PublishedTime ?? DateTime.MinValue)
            .Select(x => x.Tour)
            .ToList();
    }
}
