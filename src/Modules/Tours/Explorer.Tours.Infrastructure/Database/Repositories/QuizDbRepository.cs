using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain.Quiz;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class QuizDbRepository : IQuizRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<Quiz> _dbSet;

    public QuizDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Quiz>();
    }

    public List<Quiz> GetOwned(long authorId)
    {
        return _dbSet.Include(q => q.Questions).ThenInclude(q => q.Options).Where(q => q.AuthorId == authorId).ToList();
    }

    public Quiz? GetById(long quizId)
    {
        return _dbSet.FirstOrDefault(q => q.Id == quizId);
    }

    public Quiz GetWithDetails(long quizId)
    {
        var quiz = _dbSet.Include(q => q.Questions).ThenInclude(q => q.Options).FirstOrDefault(q => q.Id == quizId);
        if (quiz == null) throw new NotFoundException("Not found: " + quizId);
        return quiz;
    }

    public Quiz Create(Quiz quiz)
    {
        _dbSet.Add(quiz);
        _dbContext.SaveChanges();
        return quiz;
    }

    public Quiz Update(Quiz quiz)
    {
        try
        {
            DetachTrackedEntities(quiz.Id);

            _dbContext.Update(quiz);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }

        return quiz;
    }

    public void Delete(long quizId)
    {
        var quiz = GetWithDetails(quizId);
        _dbSet.Remove(quiz);
        _dbContext.SaveChanges();
    }

    public List<Quiz> GetAllForTourists()
    {
        return _dbSet.Include(q => q.Questions).ThenInclude(q => q.Options).ToList();
    }

    private void DetachTrackedEntities(long quizId)
    {
        var trackedQuizEntries = _dbContext.ChangeTracker.Entries<Quiz>()
            .Where(entry => entry.Entity.Id == quizId)
            .ToList();
        foreach (var entry in trackedQuizEntries)
        {
            entry.State = EntityState.Detached;
        }

        var trackedQuestionEntries = _dbContext.ChangeTracker.Entries<QuizQuestion>()
            .Where(entry => entry.Entity.QuizId == quizId)
            .ToList();
        foreach (var entry in trackedQuestionEntries)
        {
            entry.State = EntityState.Detached;
        }

        var trackedQuestionIds = trackedQuestionEntries.Select(q => q.Entity.Id).ToHashSet();
        var trackedOptions = _dbContext.ChangeTracker.Entries<QuizAnswerOption>()
            .Where(entry => entry.Entity.QuestionId != 0 && trackedQuestionIds.Contains(entry.Entity.QuestionId))
            .ToList();
        foreach (var entry in trackedOptions)
        {
            entry.State = EntityState.Detached;
        }
    }
}
