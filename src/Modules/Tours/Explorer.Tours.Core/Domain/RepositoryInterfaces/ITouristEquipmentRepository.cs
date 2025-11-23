using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITouristEquipmentRepository
{
    PagedResult<TouristEquipment> GetOwned(long personId, int page, int pageSize);
    TouristEquipment Create(TouristEquipment entity);
    void Delete(long personId, long equipmentId);
}
