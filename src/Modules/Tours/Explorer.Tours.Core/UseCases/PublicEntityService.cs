using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases;
    public class PublicEntityService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IFacilityRepository _facilityRepository;
        private readonly IMapper _mapper;

        public PublicEntityService(ITourRepository tourRepository, IFacilityRepository facilityRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _facilityRepository = facilityRepository;
            _mapper = mapper;
        }

        public PublicEntityDto GetAllPublic()
        {
            var facilities = _facilityRepository.GetAll().Where(f => f.IsPublic).ToList();

            var tours = _tourRepository.GetAll();

            var keyPoints = tours.Where(t => t.KeyPoints != null).SelectMany(t => t.KeyPoints!).Where(kp => kp.IsPublic).ToList();

            return new PublicEntityDto()
            {
                Facilities = facilities.Select(_mapper.Map<PublicFacilityDto>).ToList(),
                KeyPoints = keyPoints.Select(_mapper.Map<PublicKeyPointDto>).ToList()
            };
        }

        public PublicEntityDto SearchEntity(double minLon, double minLat, double maxLon, double maxLat)
        {
            var all = GetAllPublic();

            all.Facilities = all.Facilities
                .Where(f => f.Longitude >= minLon && f.Longitude <= maxLon)
                .Where(f => f.Latitude >= minLat && f.Latitude <= maxLat)
                .ToList();

            all.KeyPoints = all.KeyPoints
                .Where(kp => kp.Longitude >= minLon && kp.Longitude <= maxLon)
                .Where(kp => kp.Latitude >= minLat && kp.Latitude <= maxLat)
                .ToList();

            return all;
    }
    }

