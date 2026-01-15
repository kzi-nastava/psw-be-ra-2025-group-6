using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubPostDbRepository : IClubPostRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<ClubPost> _dbSet;

        public ClubPostDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ClubPost>();
        }

        public ClubPost Create(ClubPost post)
        {
            _dbSet.Add(post);
            _dbContext.SaveChanges();
            return post;
        }

        public void Delete(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _dbContext.SaveChanges();
            }
        }

        public ClubPost Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException("Not found: " + id);
            return entity;
        }

        public List<ClubPost> GetAllForClub(long clubId)
        {
            return _dbSet.Where(p => p.ClubId == clubId).ToList();
        }

        public ClubPost Update(ClubPost post)
        {
            var entityToUpdate = _dbSet.Find(post.Id);
            if (entityToUpdate == null)
            {
                throw new KeyNotFoundException("ClubPost not found with id: " + post.Id);
            }
            
            entityToUpdate.Update(post.Text, post.ResourceId, post.ResourceType, post.UpdatedAt);

            _dbContext.SaveChanges();

            return entityToUpdate;
        }
    }
}
