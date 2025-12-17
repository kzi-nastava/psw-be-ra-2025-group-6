using Explorer.BuildingBlocks.Core.UseCases;
namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IFacilityRepository
{
    PagedResult<Facility> GetPaged(int page, int pageSize);
    Facility Get(long id);
    List<Facility> GetAll();
    Facility Create(Facility map);
    Facility Update(Facility map);
    void Delete(long Id);
    Facility GetUntracked(long id);
}

