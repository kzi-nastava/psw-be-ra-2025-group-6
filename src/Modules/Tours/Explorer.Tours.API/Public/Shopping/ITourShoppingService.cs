using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Shopping;

public interface ITourShoppingService
{
    List<TourDto> SearchByDistance(double latitude, double longitude, double radiusInKm);
}
