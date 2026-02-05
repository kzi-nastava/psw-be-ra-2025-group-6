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
        _logger.LogDebug("TourRecommendations: touristId={TouristId} preferencesFound={Found}", touristId, preferences != null);
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
        var scored = tourDtos
            .Select(t => new
            {
                Tour = t,
                Score = TourRecommendationScoring.CalculateScore(t, preferences)
            })
            .Where(x => x.Score >= TourRecommendationScoring.RecommendationThreshold)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Tour.PublishedTime ?? DateTime.MinValue)
            .Take(limit)
            .Select(x => x.Tour)
            .ToList();

        return scored;
    }
}
