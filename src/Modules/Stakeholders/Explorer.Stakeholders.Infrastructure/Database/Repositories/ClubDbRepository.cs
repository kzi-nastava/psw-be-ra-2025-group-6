using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubDbRepository : IClubRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<Club> _dbSet;
    public ClubDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Club>();
        }

        public Club Create(Club club)
        {
            _dbSet.Add(club);
            _dbContext.SaveChanges();
            return club;
        }

        public void Delete(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException("Not found: " + id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public Club Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new KeyNotFoundException("Not found: " + id);
            return entity;
        }

        public List<Club> GetAll()
        {
            return _dbSet.ToList();
        }
        public Club Update(Club club)
        {
            var entityToUpdate = _dbSet.Find(club.Id);
            if (entityToUpdate == null)
            {
                throw new DbUpdateConcurrencyException("Club not found with id: " + club.Id);
            }
            entityToUpdate.Update(club.Name, club.Description, club.ImageUris);

            _dbContext.SaveChanges();

            return entityToUpdate;
        }
    }
}