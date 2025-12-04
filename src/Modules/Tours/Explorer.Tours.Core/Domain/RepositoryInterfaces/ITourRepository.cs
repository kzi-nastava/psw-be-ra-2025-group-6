using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    PagedResult<Tour> GetPaged(int page, int pageSize);
    List<Tour> GetAll();
    List<Tour> GetAllWithKeyPoints();
    List<Tour> GetPublishedWithKeyPoints();
    Tour Get(long id);
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    void Delete(long id);
}
