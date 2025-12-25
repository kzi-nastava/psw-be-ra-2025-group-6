using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface ITourPurchaseTokenRepository
{
    List<TourPurchaseToken> GetByTouristId(long touristId);
    TourPurchaseToken Create(TourPurchaseToken token);
    List<TourPurchaseToken> CreateBulk(List<TourPurchaseToken> tokens);
    TourPurchaseToken GetByTouristAndTour(long touristId, long tourId);
    TourPurchaseToken? GetUnusedByTouristAndTour(long touristId, long tourId);
    TourPurchaseToken Update(TourPurchaseToken token);
}
