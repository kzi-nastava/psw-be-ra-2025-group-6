using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IMonumentRepository
{
    PagedResult<Monument> GetPaged(int page, int pageSize);
    Monument Get(long id);
    Monument Create(Monument monument);
    Monument Update(Monument monument);
    void Delete(long id);
}

