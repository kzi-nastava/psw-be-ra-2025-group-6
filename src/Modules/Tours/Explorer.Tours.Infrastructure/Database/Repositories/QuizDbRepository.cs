using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain.Quiz;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        var existing = _dbSet.Include(q => q.Questions).ThenInclude(q => q.Options).FirstOrDefault(q => q.Id == quiz.Id);
        if (existing == null) throw new NotFoundException("Quiz not found.");

        existing.Title = quiz.Title;
        existing.Description = quiz.Description;
        existing.UpdatedAt = quiz.UpdatedAt;

        var incomingQuestions = quiz.Questions ?? new List<QuizQuestion>();
        var incomingQuestionIds = incomingQuestions.Select(q => q.Id).ToHashSet();

        // Remove questions missing from payload
        var questionsToRemove = existing.Questions.Where(q => !incomingQuestionIds.Contains(q.Id)).ToList();
        foreach (var question in questionsToRemove)
        {
            _dbContext.Remove(question);
        }

        // Upsert questions and their options
        foreach (var incomingQuestion in incomingQuestions)
        {
            var storedQuestion = existing.Questions.FirstOrDefault(q => q.Id == incomingQuestion.Id);
            if (storedQuestion == null)
            {
                // New question
                incomingQuestion.QuizId = existing.Id;
                _dbContext.Add(incomingQuestion);
                continue;
            }

            storedQuestion.Text = incomingQuestion.Text;
            storedQuestion.AllowsMultipleAnswers = incomingQuestion.AllowsMultipleAnswers;

            var incomingOptions = incomingQuestion.Options ?? new List<QuizAnswerOption>();
            var incomingOptionIds = incomingOptions.Select(o => o.Id).ToHashSet();

            var storedOptions = storedQuestion.Options ?? new List<QuizAnswerOption>();
            var optionsToRemove = storedOptions.Where(o => !incomingOptionIds.Contains(o.Id)).ToList();
            foreach (var option in optionsToRemove)
            {
                _dbContext.Remove(option);
            }

            foreach (var incomingOption in incomingOptions)
            {
                var storedOption = storedOptions.FirstOrDefault(o => o.Id == incomingOption.Id);
                if (storedOption == null)
                {
                    incomingOption.QuestionId = storedQuestion.Id;
                    _dbContext.Add(incomingOption);
                    continue;
                }

                storedOption.Text = incomingOption.Text;
                storedOption.Feedback = incomingOption.Feedback;
                storedOption.IsCorrect = incomingOption.IsCorrect;
            }
        }

        _dbContext.SaveChanges();
        return existing;
    }

    public void Delete(long quizId)
    {
        var quiz = GetWithDetails(quizId);
        _dbSet.Remove(quiz);
        _dbContext.SaveChanges();
    }

    public void DeleteQuestion(long quizId, long questionId)
    {
        var quiz = GetWithDetails(quizId);
        var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId && q.QuizId == quizId);
        if (question == null) throw new NotFoundException("Question not found.");

        _dbContext.Remove(question);
        _dbContext.SaveChanges();
    }

    public void DeleteOption(long quizId, long questionId, long optionId)
    {
        var quiz = GetWithDetails(quizId);
        var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId && q.QuizId == quizId);
        if (question == null) throw new NotFoundException("Question not found.");

        var option = question.Options.FirstOrDefault(o => o.Id == optionId && o.QuestionId == questionId);
        if (option == null) throw new NotFoundException("Option not found.");

        _dbContext.Remove(option);
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
