using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IEquipmentRepository
{
    PagedResult<Equipment> GetPaged(int page, int pageSize);
    Equipment Get(long id);
    Equipment Create(Equipment map);
    Equipment Update(Equipment map);
    void Delete(long id);
}