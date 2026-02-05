using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Encounters.Tests.Integration;

[Collection("Sequential")]
public class QuizEncounterIntegrationTests : BaseEncountersIntegrationTest
{
    public QuizEncounterIntegrationTests(EncountersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_quiz_encounter_with_multiple_correct_answers()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        var dto = new CreateQuizEncounterDto
        {
            ChallengeId = -201, // From e-quiz-encounters.sql seed data (Quiz type with KeyPointId)
            MinimumCorrectAnswers = 1, // Only 1 question, so minimum must be 1
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Which cities are capitals?",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "Paris", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "London", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "New York", IsCorrect = false },
                        new CreateEncounterQuizAnswerOptionDto { Text = "Los Angeles", IsCorrect = false }
                    }
                }
            }
        };

        // Act
        var result = service.CreateForKeyPoint(dto, -11); // AuthorId -11 matches CreatorId in seed

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.ChallengeId.ShouldBe(-201);
        result.MinimumCorrectAnswers.ShouldBe(1);
        result.Questions.Count.ShouldBe(1);
        result.Questions[0].AnswerOptions.Count.ShouldBe(4);
        result.Questions[0].AnswerOptions.Count(o => o.IsCorrect).ShouldBe(2); // Multiple correct answers in one question
    }

    [Fact]
    public void Creates_quiz_with_audio_story()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        var dto = new CreateQuizEncounterDto
        {
            ChallengeId = -202, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 1,
            AudioStoryPath = "/uploads/quiz-audio/test-story.mp3",
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "What did you hear in the story?",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A fortress", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "A castle", IsCorrect = false }
                    }
                }
            }
        };

        // Act
        var result = service.CreateForKeyPoint(dto, -11);

        // Assert
        result.ShouldNotBeNull();
        result.AudioStoryPath.ShouldBe("/uploads/quiz-audio/test-story.mp3");
    }

    [Fact]
    public void Gets_quiz_for_tourist_with_hidden_answers()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        // First create a quiz
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = -203, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 1,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Test question?",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "Correct", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "Wrong", IsCorrect = false }
                    }
                }
            }
        };
        service.CreateForKeyPoint(createDto, -11);

        // Act - Get for tourist (should hide correct answers)
        var result = service.GetForTourist(-203);

        // Assert
        result.ShouldNotBeNull();
        result.Questions.ShouldNotBeNull();
        result.Questions.Count.ShouldBe(1);
        
        // All answers should be marked as false for tourists
        result.Questions[0].AnswerOptions.ShouldAllBe(opt => opt.IsCorrect == false);
    }

    [Fact]
    public void Submits_quiz_successfully_with_correct_answers()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<EncountersContext>();
        
        // Create quiz
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = -204, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 2,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Question 1",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "B", IsCorrect = false }
                    }
                },
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Question 2",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "X", IsCorrect = false },
                        new CreateEncounterQuizAnswerOptionDto { Text = "Y", IsCorrect = true }
                    }
                }
            }
        };
        var quiz = service.CreateForKeyPoint(createDto, -11);

        // Submit quiz
        var submissionDto = new QuizSubmissionDto
        {
            ChallengeId = -204,
            Answers = new List<QuestionAnswerSubmissionDto>
            {
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[0].Id,
                    SelectedOptionIndexes = new List<int> { 0 } // Correct
                },
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[1].Id,
                    SelectedOptionIndexes = new List<int> { 1 } // Correct
                }
            }
        };

        // Act
        var result = service.SubmitQuiz(submissionDto, -21); // UserId -21 has XP profile in seed

        // Assert
        result.ShouldNotBeNull();
        result.CorrectAnswersCount.ShouldBe(2);
        result.TotalQuestionsCount.ShouldBe(2);
        result.IsSuccessful.ShouldBeTrue();
        result.XpAwarded.ShouldBeGreaterThan(0);
        result.QuestionResults.Count.ShouldBe(2);
        result.QuestionResults.ShouldAllBe(qr => qr.IsCompletelyCorrect);
    }

    [Fact]
    public void Submits_quiz_with_partial_correct_answers()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = -205, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 2,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Q1",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "B", IsCorrect = false }
                    }
                },
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Q2",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "X", IsCorrect = false },
                        new CreateEncounterQuizAnswerOptionDto { Text = "Y", IsCorrect = true }
                    }
                }
            }
        };
        var quiz = service.CreateForKeyPoint(createDto, -11);

        var submissionDto = new QuizSubmissionDto
        {
            ChallengeId = -205,
            Answers = new List<QuestionAnswerSubmissionDto>
            {
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[0].Id,
                    SelectedOptionIndexes = new List<int> { 0 } // Correct
                },
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[1].Id,
                    SelectedOptionIndexes = new List<int> { 0 } // Wrong
                }
            }
        };

        // Act
        var result = service.SubmitQuiz(submissionDto, -21);

        // Assert
        result.CorrectAnswersCount.ShouldBe(1);
        result.TotalQuestionsCount.ShouldBe(2);
        result.IsSuccessful.ShouldBeFalse(); // Only 1 correct, need 2
        result.XpAwarded.ShouldBe(0); // No XP for failed quiz
    }

    [Fact]
    public void Allows_quiz_retry_after_failure()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = -206, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 1,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Test",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "B", IsCorrect = false }
                    }
                }
            }
        };
        var quiz = service.CreateForKeyPoint(createDto, -11);

        // First attempt - fail
        var failSubmission = new QuizSubmissionDto
        {
            ChallengeId = -206,
            Answers = new List<QuestionAnswerSubmissionDto>
            {
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[0].Id,
                    SelectedOptionIndexes = new List<int> { 1 } // Wrong
                }
            }
        };
        var firstResult = service.SubmitQuiz(failSubmission, -21);
        firstResult.IsSuccessful.ShouldBeFalse();

        // Second attempt - succeed
        var successSubmission = new QuizSubmissionDto
        {
            ChallengeId = -206,
            Answers = new List<QuestionAnswerSubmissionDto>
            {
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[0].Id,
                    SelectedOptionIndexes = new List<int> { 0 } // Correct
                }
            }
        };

        // Act - Should allow retry
        var secondResult = service.SubmitQuiz(successSubmission, -21);

        // Assert
        secondResult.IsSuccessful.ShouldBeTrue();
        secondResult.XpAwarded.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Prevents_quiz_resubmission_after_success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = -207, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 1,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Test",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "B", IsCorrect = false }
                    }
                }
            }
        };
        var quiz = service.CreateForKeyPoint(createDto, -11);

        var submissionDto = new QuizSubmissionDto
        {
            ChallengeId = -207,
            Answers = new List<QuestionAnswerSubmissionDto>
            {
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[0].Id,
                    SelectedOptionIndexes = new List<int> { 0 } // Correct
                }
            }
        };

        // First successful submission
        service.SubmitQuiz(submissionDto, -21);

        // Act & Assert - Second attempt should fail
        Should.Throw<ArgumentException>(() =>
        {
            service.SubmitQuiz(submissionDto, -21);
        }).Message.ShouldContain("already completed");
    }

    [Fact]
    public void Updates_quiz_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = -208, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 1,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Original question",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "B", IsCorrect = false }
                    }
                }
            }
        };
        var created = service.CreateForKeyPoint(createDto, -11);

        var updateDto = new UpdateQuizEncounterDto
        {
            Id = created.Id,
            MinimumCorrectAnswers = 1, // Still only 1 question
            AudioStoryPath = "/uploads/new-audio.mp3",
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Updated question",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "X", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "Y", IsCorrect = false }
                    }
                }
            }
        };

        // Act
        var result = service.Update(updateDto, -11);

        // Assert
        result.MinimumCorrectAnswers.ShouldBe(1);
        result.AudioStoryPath.ShouldBe("/uploads/new-audio.mp3");
        result.Questions[0].Text.ShouldBe("Updated question");
    }

    [Fact]
    public void Deletes_quiz_encounter()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = -209, // From e-quiz-encounters.sql seed data
            MinimumCorrectAnswers = 1,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "To be deleted",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "B", IsCorrect = false }
                    }
                }
            }
        };
        var created = service.CreateForKeyPoint(createDto, -11);

        // Act
        service.Delete(created.Id, -11);

        // Assert - Should throw NotFoundException
        Should.Throw<Exception>(() =>
        {
            service.GetById(created.Id);
        });
    }

    [Fact]
    public void Checks_quiz_completion_status()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IQuizEncounterService>();
        
        long challengeId = -210; // From e-quiz-encounters.sql seed data
        long userId = -21; // Has XP profile in seed

        // Initially not completed
        var initialStatus = service.HasTouristCompletedQuiz(userId, challengeId);
        initialStatus.ShouldBeFalse();

        // Create and complete quiz
        var createDto = new CreateQuizEncounterDto
        {
            ChallengeId = challengeId,
            MinimumCorrectAnswers = 1,
            AudioStoryPath = null,
            Questions = new List<CreateEncounterQuizQuestionDto>
            {
                new CreateEncounterQuizQuestionDto
                {
                    Text = "Test",
                    AnswerOptions = new List<CreateEncounterQuizAnswerOptionDto>
                    {
                        new CreateEncounterQuizAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateEncounterQuizAnswerOptionDto { Text = "B", IsCorrect = false }
                    }
                }
            }
        };
        var quiz = service.CreateForKeyPoint(createDto, -11);

        var submissionDto = new QuizSubmissionDto
        {
            ChallengeId = challengeId,
            Answers = new List<QuestionAnswerSubmissionDto>
            {
                new QuestionAnswerSubmissionDto
                {
                    QuestionId = quiz.Questions[0].Id,
                    SelectedOptionIndexes = new List<int> { 0 }
                }
            }
        };
        service.SubmitQuiz(submissionDto, userId);

        // Act - Check completion status after submission
        var finalStatus = service.HasTouristCompletedQuiz(userId, challengeId);

        // Assert
        finalStatus.ShouldBeTrue();
    }
}
