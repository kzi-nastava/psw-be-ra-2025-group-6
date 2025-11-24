using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface ITouristEquipmentService
{
    PagedResult<TouristEquipmentDto> GetOwned(long personId, int page, int pageSize);
    TouristEquipmentDto Add(TouristEquipmentDto dto);
    void Remove(long personId, long equipmentId);
    PagedResult<EquipmentDto> GetAvailableEquipment(int page, int pageSize);
}
