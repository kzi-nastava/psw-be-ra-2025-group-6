using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Shopping;

public class TourShoppingService : ITourShoppingService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourShoppingService(ITourRepository tourRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public List<TourDto> SearchByDistance(double latitude, double longitude, double radiusInKm)
    {
        ValidateInputs(latitude, longitude, radiusInKm);

        var tours = _tourRepository.GetPublishedWithKeyPoints();
        var matchingTours = tours
            .Select(t => new { Tour = t, Distance = t.DistanceTo(latitude, longitude) })
            .Where(t => t.Distance <= radiusInKm)
            .OrderBy(t => t.Distance)
            .Select(t => _mapper.Map<TourDto>(t.Tour))
            .ToList();

        return matchingTours;
    }

    private static void ValidateInputs(double latitude, double longitude, double radiusInKm)
    {
        if (latitude < -90 || latitude > 90) throw new ArgumentException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180) throw new ArgumentException("Longitude must be between -180 and 180.");
        if (radiusInKm <= 0) throw new ArgumentException("Radius must be greater than 0.");
    }
}
