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
        var existing = _repository.GetById(quizId);
        if (existing is null) throw new NotFoundException("Quiz not found.");
        if (existing.AuthorId != authorId) throw new NotFoundException("Quiz not found.");


        _repository.Delete(quizId);
    }

    public List<QuizDto> GetAllForTourists()
    {
        return _mapper.Map<List<QuizDto>>(_repository.GetAllForTourists());
    }

    public QuizEvaluationResultDto SubmitAnswers(SubmitQuizAnswersDto submission, long touristId)
    {
        var quiz = _repository.GetWithDetails(submission.QuizId);
        var submittedAnswers = submission.Answers ?? new List<QuestionAnswerDto>();
        var questionResults = new List<QuestionEvaluationResultDto>();

        foreach (var question in quiz.Questions)
        {
            var submitted = submittedAnswers.FirstOrDefault(a => a.QuestionId == question.Id);
            var selected = submitted?.SelectedOptionIds ?? new List<long>();
            var correctOptions = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
            var isCompletelyCorrect = selected.Count == correctOptions.Count && !correctOptions.Except(selected).Any() && !selected.Except(correctOptions).Any();

            var optionEvaluations = question.Options.Select(option => new OptionEvaluationDto
            {
                OptionId = option.Id,
                Text = option.Text,
                IsCorrect = option.IsCorrect,
                IsSelected = selected.Contains(option.Id),
                Feedback = option.Feedback
            }).ToList();

            questionResults.Add(new QuestionEvaluationResultDto
            {
                QuestionId = question.Id,
                IsCompletelyCorrect = isCompletelyCorrect,
                Options = optionEvaluations
            });
        }

        return new QuizEvaluationResultDto
        {
            QuizId = quiz.Id,
            Questions = questionResults
        };
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