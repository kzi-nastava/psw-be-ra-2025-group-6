using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Admin;
public interface IAnnualAwardService
{
    PagedResult<AnnualAwardDto> GetPaged(int page, int pageSize);
    AnnualAwardDto Get(long id);
    AnnualAwardDto Create(AnnualAwardDto dto);
    AnnualAwardDto Update(AnnualAwardDto dto);
    void Delete(long id);
}
