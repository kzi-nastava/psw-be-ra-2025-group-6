using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class QuizCompletionDbRepository : IQuizCompletionRepository
    {
        private readonly EncountersContext _context;

        public QuizCompletionDbRepository(EncountersContext context)
        {
            _context = context;
        }

        public QuizCompletion? GetByUserAndChallenge(long userId, long challengeId)
        {
            return _context.QuizCompletions
                .FirstOrDefault(qc => qc.UserId == userId && qc.ChallengeId == challengeId);
        }

        public QuizCompletion Create(QuizCompletion quizCompletion)
        {
            _context.QuizCompletions.Add(quizCompletion);
            _context.SaveChanges();
            return quizCompletion;
        }

        public List<QuizCompletion> GetByUserId(long userId)
        {
            return _context.QuizCompletions
                .Where(qc => qc.UserId == userId)
                .ToList();
        }

        public List<QuizCompletion> GetByChallengeId(long challengeId)
        {
            return _context.QuizCompletions
                .Where(qc => qc.ChallengeId == challengeId)
                .ToList();
        }

        public void Delete(long id)
        {
            var quizCompletion = _context.QuizCompletions.Find(id);
            if (quizCompletion != null)
            {
                _context.QuizCompletions.Remove(quizCompletion);
                _context.SaveChanges();
            }
        }
    }
}
