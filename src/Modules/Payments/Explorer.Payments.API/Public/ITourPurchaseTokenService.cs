using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface ITourPurchaseTokenService
{
    List<TourPurchaseTokenDto> GetByTouristId(long touristId);
    TourPurchaseTokenDto Create(TourPurchaseTokenDto token);
    List<TourPurchaseTokenDto> CreateBulk(List<TourPurchaseTokenDto> tokens);
    TourPurchaseTokenDto? GetUnusedByTouristAndTour(long touristId, long tourId);
    TourPurchaseTokenDto MarkAsUsed(long tokenId);
}
