using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Tours.API.Public;

public interface ITourRecommendationService
{
    List<TourDto> GetRecommended(long touristId, int limit);
    TourRecommendationSummary GetRecommendationSummary(long touristId, int? limit = null);
}
