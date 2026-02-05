using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IQuizCompletionRepository
    {
        QuizCompletion? GetByUserAndChallenge(long userId, long challengeId);
        QuizCompletion Create(QuizCompletion quizCompletion);
        void Delete(long id);
        List<QuizCompletion> GetByUserId(long userId);
        List<QuizCompletion> GetByChallengeId(long challengeId);
    }
}
