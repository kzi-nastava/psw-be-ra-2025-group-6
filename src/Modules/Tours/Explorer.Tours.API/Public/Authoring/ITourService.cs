using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface ITourService
{
    List<TourDto> GetAll();
    PagedResult<TourDto> GetPaged(int page, int pageSize);
    TourDto Get(long id);
    TourDto Create(TourDto tour);
    TourDto Update(TourDto tour);
    void Delete(long id);
    TourDto Archive(long tourId, long authorId);
    TourDto Activate(long tourId, long authorId);

    void AddEquipmentToTour(long tourId, long equipmentId);
    void RemoveEquipmentFromTour(long tourId, long equipmentId);

    TourDto AddKeyPoint(long tourId, KeyPointDto keyPoint);
    TourDto UpdateTourDistance(long tourId, double distance);
    TourDto UpdateDuration(long tourId, List<TourDurationDto> durations);
}
