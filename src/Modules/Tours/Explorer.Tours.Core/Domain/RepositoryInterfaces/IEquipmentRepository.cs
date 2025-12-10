using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IEquipmentRepository
{
    List<Equipment> GetAll();
    PagedResult<Equipment> GetPaged(int page, int pageSize);
    Equipment Get(long id);
    Equipment Create(Equipment map);
    Equipment Update(Equipment map);
    void Delete(long id);
    PagedResult<Equipment> GetPagedExcluding(IEnumerable<long> excludeIds, int page, int pageSize);
}