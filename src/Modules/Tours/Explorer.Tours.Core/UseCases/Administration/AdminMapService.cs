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
        // private readonly IFacilityRepository _facilityRepository;

        public AdminMapService(IMonumentRepository monumentRepo/*, IFacilityRepository facilityRepo*/)
        {
            _monumentRepository = monumentRepo;
            // _facilityRepository = facilityRepo;
        }

        public List<AdminMapDto> GetAllMapItems()
        {
            var result = new List<AdminMapDto>();

            //Monuments deo
            var monuments = _monumentRepository.GetPaged(0, int.MaxValue).Results;
            result.AddRange(monuments.Select(m => new AdminMapDto
            {
                Id = m.Id,
                Type = "monument",
                Name = m.Name,
                Latitude = m.LocationLatitude,
                Longitude = m.LocationLongitude
            }));

            /* Facility deo, potencijalno potrebne minimalne izmene
            var facilities = _facilityRepository.GetPaged(0, int.MaxValue).Results;
            result.AddRange(facilities.Select(f => new AdminMapDto
            {
                Id = f.Id,
                Type = "facility",
                Name = f.Name,
                Latitude = f.LocationLatitude,
                Longitude = f.LocationLongitude
            }));

            */

            return result;
        }

        public AdminMapDto UpdateLocation(string type, long id, AdminUpdateLocationDto dto)
        {
            if (type == "monument")
            {
                var m = _monumentRepository.Get(id);
                var updated = CloneWithNewCoordinates(m, dto.Latitude, dto.Longitude);
                var saved = _monumentRepository.Update(updated);
                
                return new AdminMapDto
                {
                    Id = saved.Id,
                    Type = "monument",
                    Name = saved.Name,
                    Latitude = saved.LocationLatitude,
                    Longitude = saved.LocationLongitude
                };
            }
            //TODO: Facility deo
            throw new ArgumentException("Invalid type");
        }

        private Monument CloneWithNewCoordinates(Monument m, double lat, double lon)
        {
            var newM = new Monument(
                m.Name,
                lat,
                lon,
                m.Description,
                m.YearOfOrigin
            );

            typeof(Entity)
                .GetProperty("Id")!
                .SetValue(newM, m.Id);

            newM.SetStatus(m.Status);

            return newM;
        }
        //TODO: CloneWithNewCoordinates za Facility
    }
}
