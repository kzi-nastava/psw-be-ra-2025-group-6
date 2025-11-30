using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using DomainQuiz = Explorer.Tours.Core.Domain.Quiz.Quiz;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _repository;
    private readonly IMapper _mapper;
    private readonly QuizEvaluator _quizEvaluator = new();

    public QuizService(IQuizRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public List<QuizDto> GetOwned(long authorId)
    {
        return _mapper.Map<List<QuizDto>>(_repository.GetOwned(authorId));
    }

    public QuizDto Create(QuizDto quizDto, long authorId)
    {
        quizDto.AuthorId = authorId;
        quizDto.CreatedAt = DateTime.UtcNow;
        ValidateQuizDto(quizDto);
        EnsureIdentifiers(quizDto);

        var quiz = _mapper.Map<DomainQuiz>(quizDto);
        var created = _repository.Create(quiz);
        return _mapper.Map<QuizDto>(created);
    }

    public QuizDto Update(QuizDto quizDto, long authorId)
    {
        var existing = _repository.GetWithDetails(quizDto.Id);
        if (existing == null || existing.AuthorId != authorId) throw new NotFoundException("Quiz not found.");

        // Remove questions not present in payload to align persisted quiz with editor state.
        var dtoQuestionIds = (quizDto.Questions ?? new List<QuizQuestionDto>()).Select(q => q.Id).ToHashSet();
        var questionsToRemove = existing.Questions.Where(q => !dtoQuestionIds.Contains(q.Id)).ToList();
        foreach (var question in questionsToRemove)
        {
            _repository.DeleteQuestion(quizDto.Id, question.Id);
        }

        // Remove options not present in payload for remaining questions.
        foreach (var dtoQuestion in quizDto.Questions ?? new List<QuizQuestionDto>())
        {
            if (dtoQuestion.Id <= 0) continue; // new question; no existing options to prune

            var existingQuestion = existing.Questions.FirstOrDefault(q => q.Id == dtoQuestion.Id);
            if (existingQuestion == null) throw new NotFoundException("Question not found.");

            var dtoOptionIds = (dtoQuestion.Options ?? new List<QuizAnswerOptionDto>()).Select(o => o.Id).ToHashSet();
            var optionsToRemove = existingQuestion.Options.Where(o => !dtoOptionIds.Contains(o.Id)).ToList();
            foreach (var option in optionsToRemove)
            {
                _repository.DeleteOption(quizDto.Id, dtoQuestion.Id, option.Id);
            }
        }

        quizDto.AuthorId = authorId;
        quizDto.CreatedAt = existing.CreatedAt;
        quizDto.UpdatedAt = DateTime.UtcNow;
        ValidateQuizDto(quizDto);
        EnsureIdentifiers(quizDto);

        var updated = _mapper.Map<DomainQuiz>(quizDto);
        var result = _repository.Update(updated);
        return _mapper.Map<QuizDto>(result);
    }

    public void Delete(long quizId, long authorId)
    {
        var existing = _repository.GetWithDetails(quizId);
        if (existing.AuthorId != authorId) throw new ForbiddenException("Quiz not owned by author.");

        _repository.Delete(quizId);
    }

    public void DeleteQuestion(long quizId, long questionId, long authorId)
    {
        var quiz = _repository.GetWithDetails(quizId);
        if (quiz.AuthorId != authorId) throw new ForbiddenException("Quiz not owned by author.");

        if (quiz.Questions.All(q => q.Id != questionId)) throw new NotFoundException("Question not found.");

        _repository.DeleteQuestion(quizId, questionId);
    }

    public void DeleteOption(long quizId, long questionId, long optionId, long authorId)
    {
        var quiz = _repository.GetWithDetails(quizId);
        if (quiz.AuthorId != authorId) throw new ForbiddenException("Quiz not owned by author.");

        var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId);
        if (question == null) throw new NotFoundException("Question not found.");
        if (question.Options.All(o => o.Id != optionId)) throw new NotFoundException("Option not found.");

        _repository.DeleteOption(quizId, questionId, optionId);
    }

    public List<QuizDto> GetAllForTourists()
    {
        return _mapper.Map<List<QuizDto>>(_repository.GetAllForTourists());
    }

    public QuizEvaluationResultDto SubmitAnswers(SubmitQuizAnswersDto submission, long touristId)
    {
        var quiz = _repository.GetWithDetails(submission.QuizId);
        return _quizEvaluator.Evaluate(quiz, submission);
    }

        private static void ValidateQuizDto(QuizDto quizDto)
    {
        if (string.IsNullOrWhiteSpace(quizDto.Title)) throw new ArgumentException("Invalid Title.");

        foreach (var question in quizDto.Questions ?? new List<QuizQuestionDto>())
        {
            if (string.IsNullOrWhiteSpace(question.Text)) throw new ArgumentException("Invalid Question Text.");
            if (question.Options == null || question.Options.Count == 0)
                throw new ArgumentException("Question must contain answer options.");

            var correctCount = question.Options.Count(o => o.IsCorrect);
            if (correctCount == 0) throw new ArgumentException("Question must contain at least one correct answer.");
            if (!question.AllowsMultipleAnswers && correctCount != 1)
                throw new ArgumentException("Single-choice question must have exactly one correct answer.");

            foreach (var option in question.Options)
            {
                if (string.IsNullOrWhiteSpace(option.Text)) throw new ArgumentException("Invalid Option Text.");
                if (string.IsNullOrWhiteSpace(option.Feedback)) throw new ArgumentException("Invalid Feedback.");
            }
        }
    }


    private static void EnsureIdentifiers(QuizDto quizDto)
    {
        foreach (var question in quizDto.Questions ?? new List<QuizQuestionDto>())
        {
            if (question.QuizId == 0) question.QuizId = quizDto.Id;
            foreach (var option in question.Options ?? new List<QuizAnswerOptionDto>())
            {
                if (option.QuestionId == 0) option.QuestionId = question.Id;
            }
        }
    }
}
