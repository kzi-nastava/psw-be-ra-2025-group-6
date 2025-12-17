using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class FollowerDbRepository : IFollowerRepository
    {
        private readonly StakeholdersContext _dbContext;
        public FollowerDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Follower> GetAll(long UserId)
        {
            return _dbContext.Followers
               .Where(n => n.UserId == UserId)
               .ToList();
        }
    }
}
