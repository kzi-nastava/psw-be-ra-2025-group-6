using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ITourProblemMessageRepository : ICrudRepository<TourProblemMessage>
    {
        PagedResult<TourProblemMessage> GetForProblem(long problemId, int page, int pageSize);
    }
}
