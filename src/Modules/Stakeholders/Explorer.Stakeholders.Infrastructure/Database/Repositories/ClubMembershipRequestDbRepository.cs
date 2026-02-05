using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubMembershipRequestDbRepository : IClubMembershipRequestRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<ClubMembershipRequest> _dbSet;

        public ClubMembershipRequestDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ClubMembershipRequest>();
        }

        public ClubMembershipRequest Create(ClubMembershipRequest request)
        {
            _dbSet.Add(request);
            _dbContext.SaveChanges();
            return request;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public ClubMembershipRequest Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException("Membership request not found: " + id);
            return entity;
        }

        public List<ClubMembershipRequest> GetByClub(long clubId)
        {
            return _dbSet
                .Where(r => r.ClubId == clubId && r.Status == ClubMembershipRequestStatus.Processing)
                .ToList();
        }

        public ClubMembershipRequest? GetActive(long clubId, long touristId)
        {
            return _dbSet
                .FirstOrDefault(r => r.ClubId == clubId
                                  && r.TouristId == touristId
                                  && r.Status == ClubMembershipRequestStatus.Processing);
        }
    }
}