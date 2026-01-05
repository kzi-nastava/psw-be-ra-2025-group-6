using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;
  public interface IPublicEntityService
  {
      PublicEntityDto GetAllPublic();
      PublicEntityDto SearchEntities(
          double? minLon, 
          double? minLat, 
          double? maxLon, 
          double? maxLat, 
          string? query, 
          PublicEntityTypeDto? entityType, 
          FacilityType? facilityType);
  }

