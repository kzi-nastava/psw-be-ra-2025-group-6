using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface IQuizService
{
    List<QuizDto> GetOwned(long authorId);
    QuizDto Create(QuizDto quiz, long authorId);
    QuizDto Update(QuizDto quiz, long authorId);
    void Delete(long quizId, long authorId);

    List<QuizDto> GetAllForTourists();
    QuizEvaluationResultDto SubmitAnswers(SubmitQuizAnswersDto submission, long touristId);
}