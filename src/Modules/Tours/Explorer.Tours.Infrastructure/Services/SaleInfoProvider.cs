using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Internal;

namespace Explorer.Tours.Infrastructure.Services;

public class SaleInfoProvider : ISaleInfoProvider
{
    private readonly ISaleService _saleService;

    public SaleInfoProvider(ISaleService saleService)
    {
        _saleService = saleService;
    }

    public SaleInfo? GetActiveSaleForTour(long tourId)
    {
        var activeSales = _saleService.GetActiveSales();
        var saleForTour = activeSales.FirstOrDefault(s => s.TourIds.Contains(tourId));

        if (saleForTour == null)
            return null;

        return new SaleInfo
        {
            DiscountPercent = saleForTour.DiscountPercent,
            StartDate = saleForTour.StartDate,
            EndDate = saleForTour.EndDate
        };
    }
}
