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

    void AddEquipmentToTour(long tourId, long equipmentId);
}
