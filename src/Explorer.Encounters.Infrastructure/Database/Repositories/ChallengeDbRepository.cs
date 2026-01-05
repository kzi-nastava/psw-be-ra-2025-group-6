using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class ChallengeDbRepository : IChallengeRepository
    {
        private readonly EncountersContext _db;
        public ChallengeDbRepository(EncountersContext db) { _db = db; }

        public Challenge Create(Challenge challenge)
        {
            _db.Set<Challenge>().Add(challenge);
            _db.SaveChanges();
            return challenge;
        }

        public void Delete(long id)
        {
            var c = _db.Set<Challenge>().Find(id);
            if (c == null) throw new KeyNotFoundException("Not found.");
            _db.Set<Challenge>().Remove(c);
            _db.SaveChanges();
        }

        public Challenge Get(long id)
        {
            return _db.Set<Challenge>().FirstOrDefault(x => x.Id == id);
        }

        public List<Challenge> GetAllActive()
        {
            return _db.Set<Challenge>().Where(c => c.Status == ChallengeStatus.Active).ToList();
        }

        public List<Challenge> GetAll()
        {
            return _db.Set<Challenge>().ToList();
        }

        public Challenge Update(Challenge challenge)
        {
            _db.Set<Challenge>().Update(challenge);
            _db.SaveChanges();
            return challenge;
        }
    }
}
