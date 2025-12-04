using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Marketplace;

public interface ITourMarketplaceService
{
    List<TourSummaryDto> SearchByDistance(TourSearchByDistanceRequestDto request);
}
