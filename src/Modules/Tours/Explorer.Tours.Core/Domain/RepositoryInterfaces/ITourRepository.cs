using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository<Tour>  where Tour : Entity
{
    PagedResult<Tour> GetPaged(int page, int pageSize);
    Tour Get(long id);
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    void Delete(long id);
}
