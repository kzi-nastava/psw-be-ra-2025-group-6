using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class AdminMapService : IAdminMapService
    {
        private readonly IMonumentRepository _monumentRepository;
        private readonly IFacilityRepository _facilityRepository;

        public AdminMapService(IMonumentRepository monumentRepo, IFacilityRepository facilityRepo)
        {
            _monumentRepository = monumentRepo;
            _facilityRepository = facilityRepo;
        }

        public List<AdminMapDto> GetAllMapItems()
        {
            var result = new List<AdminMapDto>();

            var monuments = _monumentRepository.GetPaged(0, int.MaxValue).Results ?? new List<Monument>();
            result.AddRange(monuments.Select(m => new AdminMapDto
            {
                Id = $"monument-{m.Id}",
                Type = "monument",
                Name = m.Name,
                Latitude = m.LocationLatitude,
                Longitude = m.LocationLongitude
            }));

            var facilities = _facilityRepository.GetPaged(0, int.MaxValue).Results;
            result.AddRange(facilities.Select(f => new AdminMapDto
            {
                Id = $"facility-{f.Id}",
                Type = "facility",
                Name = f.Name,
                Latitude = f.Latitude,
                Longitude = f.Longitude
            }));

            return result;
        }

        public AdminMapDto UpdateLocation(string type, long id, AdminUpdateLocationDto dto)
        {
            if (type == "monument")
            {
                var m = _monumentRepository.GetUntracked(id);
                var updated = CloneWithNewCoordinates(m, dto.Latitude, dto.Longitude);
                var saved = _monumentRepository.Update(updated);

                return new AdminMapDto
                {
                    Id = $"monument-{saved.Id}",
                    Type = "monument",
                    Name = saved.Name,
                    Latitude = saved.LocationLatitude,
                    Longitude = saved.LocationLongitude
                };
            }
            else if (type == "facility")
            {
                var f = _facilityRepository.GetUntracked(id);
                var updated = CloneWithNewCoordinates(f, dto.Latitude, dto.Longitude);
                var saved = _facilityRepository.Update(updated);

                return new AdminMapDto
                {
                    Id = $"facility-{saved.Id}",
                    Type = "facility",
                    Name = saved.Name,
                    Latitude = saved.Latitude,
                    Longitude = saved.Longitude
                };
            }

            throw new ArgumentException("Invalid type");
        }

        private Monument CloneWithNewCoordinates(Monument m, double lat, double lon)
        {
            var newM = new Monument(
                m.Name,
                lat,
                lon,
                m.Description ?? string.Empty,
                m.YearOfOrigin ?? 0
            );

            typeof(Entity)
                .GetProperty("Id")!
                .SetValue(newM, m.Id);

            newM.SetStatus(m.Status);

            return newM;
        }

        private Facility CloneWithNewCoordinates(Facility f, double lat, double lon)
        {
            var newF = new Facility(
                f.Name,
                f.Comment,
                lon,
                lat,
                f.Type
            );

            typeof(Entity)
                .GetProperty("Id")!
                .SetValue(newF, f.Id);

            return newF;
        }
    }
}
