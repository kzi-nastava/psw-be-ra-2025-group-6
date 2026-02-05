using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IQuizEncounterRepository
    {
        QuizEncounter? Get(long id);
        QuizEncounter? GetById(long id);
        QuizEncounter? GetByChallengeId(long challengeId);
        QuizEncounter? GetByIdWithQuestions(long id);
        QuizEncounter? GetByChallengeIdWithQuestions(long challengeId);
        QuizEncounter Create(QuizEncounter quizEncounter);
        QuizEncounter Update(QuizEncounter quizEncounter);
        void Delete(long id);
        List<QuizEncounter> GetAllByAuthorId(long authorId);
        List<QuizEncounter> GetAll();
    }
}
