using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;
public interface IAnnualAwardRepository<AnnualAward> where AnnualAward : Entity
{
    PagedResult<AnnualAward> GetPaged(int page, int pageSize);
    AnnualAward Get(long id);
    AnnualAward Create(AnnualAward award);
    AnnualAward Update(AnnualAward award);
    void Delete(long id);
}

