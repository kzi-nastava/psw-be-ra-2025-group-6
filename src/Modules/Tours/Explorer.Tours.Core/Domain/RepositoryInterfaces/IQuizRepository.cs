using DomainQuiz = Explorer.Tours.Core.Domain.Quiz.Quiz;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IQuizRepository
{
    List<DomainQuiz> GetOwned(long authorId);
    DomainQuiz? GetById(long quizId);
    DomainQuiz GetWithDetails(long quizId);
    DomainQuiz Create(DomainQuiz quiz);
    DomainQuiz Update(DomainQuiz quiz);
    void Delete(long quizId);
    List<DomainQuiz> GetAllForTourists();
}