using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;
public interface ITourPurchaseTokenRepository
{
    List<TourPurchaseToken> GetByTouristId(long touristId);
    TourPurchaseToken Create(TourPurchaseToken token);
    List<TourPurchaseToken> CreateBulk(List<TourPurchaseToken> tokens);
    TourPurchaseToken GetByTouristAndTour(long touristId, long tourId);
    TourPurchaseToken? GetUnusedByTouristAndTour(long touristId, long tourId);
    TourPurchaseToken Update(TourPurchaseToken token);
}
