using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class QuizCommandTests : BaseToursIntegrationTest
{
    public QuizCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_quiz()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "3");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var dto = BuildValidQuizDto();

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as QuizDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.AuthorId.ShouldBe(3);

        var stored = dbContext.Quizzes.Include(q => q.Questions).ThenInclude(q => q.Options).FirstOrDefault(q => q.Id == result.Id);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(dto.Title);
        stored.Questions.Count.ShouldBe(1);
        stored.Questions.First().Options.Count.ShouldBe(2);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "3");
        var dto = new QuizDto
        {
            Title = "",
            Description = "Invalid",
            Questions = new List<QuizQuestionDto>()
        };

        Should.Throw<ArgumentException>(() => controller.Create(dto));
    }

    [Fact]
    public void Updates_quiz()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedDto = new QuizDto
        {
            Id = -1,
            Title = "Belgrade Basics Updated",
            Description = "Updated description",
            Questions = new List<QuizQuestionDto>
            {
                new QuizQuestionDto
                {
                    Id = -11,
                    QuizId = -1,
                    Text = "Updated river question",
                    AllowsMultipleAnswers = false,
                    Options = new List<QuizAnswerOptionDto>
                    {
                        new QuizAnswerOptionDto{ Id = -111, QuestionId = -11, Text = "Sava", IsCorrect = false, Feedback = "Still incorrect."},
                        new QuizAnswerOptionDto{ Id = -112, QuestionId = -11, Text = "Danube", IsCorrect = true, Feedback = "Still correct."}
                    }
                }
            }
        };

        var result = ((ObjectResult)controller.Update(-1, updatedDto).Result)?.Value as QuizDto;

        result.ShouldNotBeNull();
        result.Title.ShouldBe(updatedDto.Title);
        result.Description.ShouldBe(updatedDto.Description);
        result.Questions.First().Text.ShouldBe(updatedDto.Questions.First().Text);

        var stored = dbContext.Quizzes.Include(q => q.Questions).ThenInclude(q => q.Options).First(q => q.Id == -1);
        stored.Title.ShouldBe(updatedDto.Title);
        stored.Questions.First().Text.ShouldBe(updatedDto.Questions.First().Text);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "1");
        var updatedDto = BuildValidQuizDto();
        updatedDto.Id = -1000;

        Should.Throw<NotFoundException>(() => controller.Update(-1000, updatedDto));
    }

    [Fact]
    public void Deletes_quiz()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "2");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var result = (OkResult)controller.Delete(-2);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);
        dbContext.Quizzes.FirstOrDefault(q => q.Id == -2).ShouldBeNull();
        dbContext.QuizQuestions.FirstOrDefault(q => q.QuizId == -2).ShouldBeNull();
        dbContext.QuizAnswerOptions.FirstOrDefault(o => o.QuestionId == -21).ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "1");

        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    [Fact]
    public void Submits_answers_and_evaluates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "5");

        var submission = new SubmitQuizAnswersDto
        {
            QuizId = -1,
            Answers = new List<QuestionAnswerDto>
            {
                new QuestionAnswerDto{ QuestionId = -11, SelectedOptionIds = new List<long>{ -112 }},
                new QuestionAnswerDto{ QuestionId = -12, SelectedOptionIds = new List<long>{ -121, -123 }}
            }
        };

        var result = ((ObjectResult)controller.Submit(-1, submission).Result)?.Value as QuizEvaluationResultDto;

        result.ShouldNotBeNull();
        result.QuizId.ShouldBe(-1);
        result.Questions.Count.ShouldBe(2);
        result.Questions.First(q => q.QuestionId == -11).IsCompletelyCorrect.ShouldBeTrue();
        result.Questions.First(q => q.QuestionId == -12).IsCompletelyCorrect.ShouldBeTrue();
    }

    private static QuizDto BuildValidQuizDto()
    {
        return new QuizDto
        {
            Title = "New Quiz",
            Description = "Desc",
            Questions = new List<QuizQuestionDto>
            {
                new QuizQuestionDto
                {
                    Text = "Question 1",
                    AllowsMultipleAnswers = false,
                    Options = new List<QuizAnswerOptionDto>
                    {
                        new QuizAnswerOptionDto{ Text = "Correct", IsCorrect = true, Feedback = "Good choice"},
                        new QuizAnswerOptionDto{ Text = "Wrong", IsCorrect = false, Feedback = "Not correct"}
                    }
                }
            }
        };
    }

    private static QuizController CreateController(IServiceScope scope, string personId)
    {
        return new QuizController(scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}