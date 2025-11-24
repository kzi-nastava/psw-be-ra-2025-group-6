using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IMonumentService
{
    PagedResult<MonumentDto> GetPaged(int page, int pageSize);
    MonumentDto Get(long id);
    MonumentDto Create(MonumentDto monument);
    MonumentDto Update(MonumentDto monument);
    void Delete(long id);
}

