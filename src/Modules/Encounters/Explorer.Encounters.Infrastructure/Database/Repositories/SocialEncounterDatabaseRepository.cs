using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class SocialEncounterDatabaseRepository : ISocialEncounterRepository
    {
        private readonly EncountersContext _dbContext;

        public SocialEncounterDatabaseRepository(EncountersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SocialEncounter Create(SocialEncounter encounter)
        {
            _dbContext.SocialEncounters.Add(encounter);
            _dbContext.SaveChanges();
            return encounter;
        }

        public SocialEncounter Update(SocialEncounter encounter)
        {
            _dbContext.SocialEncounters.Update(encounter);
            _dbContext.SaveChanges();
            return encounter;
        }

        public SocialEncounter? Get(long id)
        {
            return _dbContext.SocialEncounters.Find(id);
        }

        public SocialEncounter? GetByChallengeId(long challengeId)
        {
            return _dbContext.SocialEncounters
                .FirstOrDefault(se => se.ChallengeId == challengeId);
        }

        public List<SocialEncounter> GetAll()
        {
            return _dbContext.SocialEncounters
                .AsNoTracking()
                .ToList();
        }

        public void Delete(long id)
        {
            var encounter = Get(id);
            if (encounter != null)
            {
                _dbContext.SocialEncounters.Remove(encounter);
                _dbContext.SaveChanges();
            }
        }
    }
}
