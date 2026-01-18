using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class TourProblemMessageDatabaseRepository : CrudDatabaseRepository<TourProblemMessage, StakeholdersContext>, ITourProblemMessageRepository
    {
        private readonly StakeholdersContext _dbContext;
        public TourProblemMessageDatabaseRepository(StakeholdersContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public PagedResult<TourProblemMessage> GetForProblem(long problemId, int page, int pageSize)
        {
            var query = _dbContext.TourProblemMessages
                .Where(m => m.TourProblemId == problemId)
                .OrderBy(m => m.Timestamp);

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<TourProblemMessage>(items, totalCount);
        }
    }
}
