using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TouristViewService : ITouristViewService
    {
        private readonly ITourRepository _tourRepository;

        public TouristViewService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }
        public List<TouristTourDto> GetPublishedTours()
        {
            var tours = _tourRepository.GetPublishedTours();
            var touristViews = tours.Select(tour => new TouristTourDto
            {
                Name = tour.Name,
                FirstKeyPoint = tour.KeyPoints?.FirstOrDefault() != null
                    ? new KeyPointDto
                    {
                        Name = tour.KeyPoints.First().Name,
                        Description = tour.KeyPoints.First().Description,
                        ImagePath = tour.KeyPoints.First().ImagePath  
                    }
                    : null,

                KeyPoints = tour.KeyPoints?.Select(kp => new KeyPointDto
                {
                    Id = kp.Id,
                    TourId = kp.TourId,
                    Name = kp.Name,
                    Description = kp.Description,
                    Longitude = kp.Longitude,
                    Latitude = kp.Latitude,
                    ImagePath = kp.ImagePath,  
                    Secret = kp.Secret
                }).ToList() ?? new List<KeyPointDto>(),

                Difficulty = (TourDifficultyDto)tour.Difficulty,
                Price = tour.Price,
                Tags = tour.Tags ?? new List<string>(),
                DistanceInKm = tour.DistanceInKm,
                Duration = tour.Duration?.Select(d => new TourDurationDto
                {
                    TravelType = (TravelTypeDto)d.TravelType,
                    Minutes = d.Minutes
                }).ToList() ?? new List<TourDurationDto>(),
                Description = tour.Description
            }).ToList();

            return touristViews;
        }
    }
}
