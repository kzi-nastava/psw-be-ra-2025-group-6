using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class HiddenLocationAttemptDbRepository : IHiddenLocationAttemptRepository
    {
        private readonly EncountersContext _db;

        public HiddenLocationAttemptDbRepository(EncountersContext db)
        {
            _db = db;
        }

        public HiddenLocationAttempt Create(HiddenLocationAttempt attempt)
        {
            _db.Set<HiddenLocationAttempt>().Add(attempt);
            _db.SaveChanges();
            return attempt;
        }

        public HiddenLocationAttempt Get(long id)
        {
            return _db.Set<HiddenLocationAttempt>().FirstOrDefault(x => x.Id == id)
                ?? throw new KeyNotFoundException($"Attempt {id} not found.");
        }

        public HiddenLocationAttempt? GetActiveAttempt(long userId, long challengeId)
        {
            return _db.Set<HiddenLocationAttempt>()
                .Where(a => a.UserId == userId 
                    && a.ChallengeId == challengeId 
                    && !a.CompletedAt.HasValue)
                .OrderByDescending(a => a.StartedAt)
                .FirstOrDefault();
        }

        public List<HiddenLocationAttempt> GetUserAttempts(long userId, long challengeId)
        {
            return _db.Set<HiddenLocationAttempt>()
                .Where(a => a.UserId == userId && a.ChallengeId == challengeId)
                .OrderByDescending(a => a.StartedAt)
                .ToList();
        }

        public HiddenLocationAttempt Update(HiddenLocationAttempt attempt)
        {
            _db.Set<HiddenLocationAttempt>().Update(attempt);
            _db.SaveChanges();
            return attempt;
        }
    }
}
