using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class SocialMessageDatabaseRepository : CrudDatabaseRepository<SocialMessage, StakeholdersContext>, ISocialMessageRepository
    {
        private readonly StakeholdersContext _dbContext;

        public SocialMessageDatabaseRepository(StakeholdersContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public PagedResult<SocialMessage> GetConversation(long userId1, long userId2, int page, int pageSize)
        {
            var query = _dbContext.SocialMessages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.Timestamp);

            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<SocialMessage>(items, totalCount);
        }
    }
}
