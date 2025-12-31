using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterCompletionDbRepository : IEncounterCompletionRepository
    {
        private readonly EncountersContext _context;

        public EncounterCompletionDbRepository(EncountersContext context)
        {
            _context = context;
        }

        public EncounterCompletion Create(EncounterCompletion completion)
        {
            _context.EncounterCompletions.Add(completion);
            _context.SaveChanges();
            return completion;
        }

        public bool HasUserCompletedChallenge(long userId, long challengeId)
        {
            return _context.EncounterCompletions
                .Any(c => c.UserId == userId && c.ChallengeId == challengeId);
        }

        public List<EncounterCompletion> GetByUserId(long userId)
        {
            return _context.EncounterCompletions
                .Where(c => c.UserId == userId)
                .ToList();
        }
    }
}
