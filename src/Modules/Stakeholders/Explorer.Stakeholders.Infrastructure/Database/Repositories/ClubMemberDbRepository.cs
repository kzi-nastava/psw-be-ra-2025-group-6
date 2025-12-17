using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubMemberDbRepository : IClubMemberRepository
    {
        private readonly StakeholdersContext _dbContext;
        public ClubMemberDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<ClubMember> GetAll(long ClubId)
        {
            return _dbContext.ClubMembers
               .Where(n => n.ClubId == ClubId)
               .ToList();
        }

    }
}