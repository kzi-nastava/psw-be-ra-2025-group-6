using Explorer.Payments.API.Internal;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Infrastructure.Services;

public class TourDataProvider : ITourDataProvider
{
    private readonly ITourRepository _tourRepository;

    public TourDataProvider(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
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
}
