using Explorer.Payments.API.Internal;

namespace Explorer.Tours.API.Internal;

public interface ISaleInfoProvider
{
    SaleInfo? GetActiveSaleForTour(long tourId);
}
