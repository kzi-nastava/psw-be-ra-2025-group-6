using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class QuizEncounterDbRepository : IQuizEncounterRepository
    {
        private readonly EncountersContext _context;

        public QuizEncounterDbRepository(EncountersContext context)
        {
            _context = context;
        }

        public QuizEncounter? Get(long id)
        {
            return GetByIdWithQuestions(id);
        }

        public QuizEncounter? GetById(long id)
        {
            return _context.QuizEncounters.FirstOrDefault(q => q.Id == id);
        }

        public QuizEncounter? GetByChallengeId(long challengeId)
        {
            return _context.QuizEncounters.FirstOrDefault(q => q.ChallengeId == challengeId);
        }

        public QuizEncounter? GetByIdWithQuestions(long id)
        {
            return _context.QuizEncounters
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.Id == id);
        }

        public QuizEncounter? GetByChallengeIdWithQuestions(long challengeId)
        {
            return _context.QuizEncounters
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.ChallengeId == challengeId);
        }

        public QuizEncounter Create(QuizEncounter quizEncounter)
        {
            _context.QuizEncounters.Add(quizEncounter);
            _context.SaveChanges();
            return quizEncounter;
        }

        public QuizEncounter Update(QuizEncounter quizEncounter)
        {
            _context.QuizEncounters.Update(quizEncounter);
            _context.SaveChanges();
            return quizEncounter;
        }

        public void Delete(long id)
        {
            var quizEncounter = GetByIdWithQuestions(id);
            if (quizEncounter != null)
            {
                _context.QuizEncounters.Remove(quizEncounter);
                _context.SaveChanges();
            }
        }

        public List<QuizEncounter> GetAllByAuthorId(long authorId)
        {
            return _context.QuizEncounters
                .Include(q => q.Questions)
                .Where(q => _context.Challenges.Any(c => c.Id == q.ChallengeId && c.CreatorId == authorId))
                .ToList();
        }

        public List<QuizEncounter> GetAll()
        {
            return _context.QuizEncounters
                .Include(q => q.Questions)
                .ToList();
        }
    }
}
