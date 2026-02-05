using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface ITourRecommendationService
{
    List<TourDto> GetRecommended(long touristId, int limit);
}
