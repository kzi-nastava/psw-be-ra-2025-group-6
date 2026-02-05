using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Public
{
    public interface IQuizEncounterService
    {
        QuizEncounterDto CreateForKeyPoint(CreateQuizEncounterDto dto, long authorId);
        QuizEncounterDto Update(UpdateQuizEncounterDto dto, long authorId);
        void Delete(long id, long authorId);
        QuizEncounterDto GetById(long id);
        QuizEncounterDto GetByChallengeId(long challengeId);
        List<QuizEncounterDto> GetByAuthor(long authorId);
        QuizEncounterDto GetForTourist(long challengeId);
        
        QuizResultDto SubmitQuiz(QuizSubmissionDto dto, long userId);
        bool HasTouristCompletedQuiz(long userId, long challengeId);
    }
}
