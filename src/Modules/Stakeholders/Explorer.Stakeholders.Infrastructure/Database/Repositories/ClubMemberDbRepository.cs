using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubMemberDbRepository : IClubMemberRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<ClubMember> _dbSet;

        public ClubMemberDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ClubMember>();
        }

        public ClubMember Create(ClubMember member)
        {
            _dbSet.Add(member);
            _dbContext.SaveChanges();
            return member;
        }

        public ClubMember Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException("Member not found: " + id);
            return entity;
        }

        public void Delete(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException("Member not found: " + id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public ClubMember? GetByClubAndUser(long clubId, long userId)
        {
            return _dbSet.FirstOrDefault(m => m.ClubId == clubId && m.UserId == userId && m.Status == ClubMemberStatus.Active);
        }

        public List<ClubMember> GetByClubId(long clubId)
        {
            return _dbSet.Where(m => m.ClubId == clubId && m.Status == ClubMemberStatus.Active).ToList();
        }

        public List<ClubMember> GetByUserId(long userId)
        {
            return _dbSet.Where(m => m.UserId == userId && m.Status == ClubMemberStatus.Active).ToList();
        }

        public bool IsMember(long clubId, long userId)
        {
            return _dbSet.Any(m => m.ClubId == clubId && m.UserId == userId && m.Status == ClubMemberStatus.Active);
        }
    }
}
