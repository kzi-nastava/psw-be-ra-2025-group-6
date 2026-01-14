using Explorer.Payments.API.Internal;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Services;

public class TourDataProvider : ITourDataProvider
{
    private readonly ITourRepository _tourRepository;
    private readonly ISaleInfoProvider _saleInfoProvider;

    public TourDataProvider(ITourRepository tourRepository, ISaleInfoProvider saleInfoProvider)
    {
        _tourRepository = tourRepository;
        _saleInfoProvider = saleInfoProvider;
    }

    public TourData GetTourData(long tourId)
    {
        var tour = _tourRepository.Get(tourId);
        return new TourData
        {
            Id = tour.Id,
            Name = tour.Name,
            Price = tour.Price,
            Status = tour.Status.ToString().ToUpper() 
        };
    }

    public bool VerifyToursOwnership(long authorId, List<long> tourIds)
    {
        foreach (var tourId in tourIds)
        {
            var tour = _tourRepository.Get(tourId);
            if (tour.AuthorId != authorId)
                return false;
        }
        return true;
    }

    public int GetPublishedToursCount(List<long> tourIds)
    {
        int count = 0;
        foreach (var tourId in tourIds)
        {
            var tour = _tourRepository.Get(tourId);
            if (tour.Status == TourStatus.CONFIRMED)
                count++;
        }
        return count;
    }

    public double GetTotalPrice(List<long> tourIds)
    {
        double totalPrice = 0;
        foreach (var tourId in tourIds)
        {
            var tour = _tourRepository.Get(tourId);
            totalPrice += tour.Price;
        }
        return totalPrice;
    }

    public SaleInfo? GetActiveSaleForTour(long tourId)
    {
        return _saleInfoProvider.GetActiveSaleForTour(tourId);
    }
}
