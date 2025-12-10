using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Marketplace;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Marketplace;

public class TourMarketplaceService : ITourMarketplaceService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourMarketplaceService(ITourRepository tourRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public List<TourSummaryDto> SearchByDistance(TourSearchByDistanceRequestDto request)
    {
        Validate(request);
        var tours = _tourRepository.GetPublishedWithKeyPoints();
        var matches = tours.Where(t => t.KeyPoints != null && t.KeyPoints.Any(kp =>
            GeoDistanceCalculator.DistanceInKilometers(
                request.Latitude, request.Longitude, kp.Latitude, kp.Longitude) <= request.DistanceInKm));

        return _mapper.Map<List<TourSummaryDto>>(matches);
    }

    private static void Validate(TourSearchByDistanceRequestDto request)
    {
        if (request.DistanceInKm <= 0) throw new ArgumentException("DistanceInKm must be positive.");
        if (request.Latitude is < -90 or > 90) throw new ArgumentException("Latitude is out of range.");
        if (request.Longitude is < -180 or > 180) throw new ArgumentException("Longitude is out of range.");
    }
}

internal static class GeoDistanceCalculator
{
    private const double EarthRadiusKm = 6371;

    public static double DistanceInKilometers(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);
        var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Pow(Math.Sin(dLon / 2), 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double angle) => Math.PI * angle / 180.0;
}
