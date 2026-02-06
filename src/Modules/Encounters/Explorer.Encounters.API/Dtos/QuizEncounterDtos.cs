namespace Explorer.Encounters.API.Dtos
{
    public class QuizEncounterDto
    {
        public long Id { get; set; }
        public long ChallengeId { get; set; }
        public int MinimumCorrectAnswers { get; set; }
        public string? AudioStoryPath { get; set; }
        public List<EncounterQuizQuestionDto> Questions { get; set; } = new();
    }

    public class EncounterQuizQuestionDto
    {
        public long Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<EncounterQuizAnswerOptionDto> AnswerOptions { get; set; } = new();
        public long QuizEncounterId { get; set; }
    }

    public class EncounterQuizAnswerOptionDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class QuizCompletionDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long QuizEncounterId { get; set; }
        public long ChallengeId { get; set; }
        public DateTime CompletedAt { get; set; }
        public int CorrectAnswersCount { get; set; }
        public int TotalQuestionsCount { get; set; }
        public bool IsSuccessful { get; set; }
        public int XpAwarded { get; set; }
    }

    public class CreateQuizEncounterDto
    {
        public long ChallengeId { get; set; }
        public int MinimumCorrectAnswers { get; set; }
        public string? AudioStoryPath { get; set; }
        public List<CreateEncounterQuizQuestionDto> Questions { get; set; } = new();
    }

    public class UpdateQuizEncounterDto
    {
        public long Id { get; set; }
        public long ChallengeId { get; set; }
        public int MinimumCorrectAnswers { get; set; }
        public string? AudioStoryPath { get; set; }
        public List<CreateEncounterQuizQuestionDto> Questions { get; set; } = new();
    }

    public class CreateEncounterQuizQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public List<CreateEncounterQuizAnswerOptionDto> AnswerOptions { get; set; } = new();
    }

    public class CreateEncounterQuizAnswerOptionDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class SubmitQuizAnswersDto
    {
        public long QuizEncounterId { get; set; }
        public long ChallengeId { get; set; }
        public List<QuestionAnswerSubmissionDto> Answers { get; set; } = new();
    }

    public class QuestionAnswerSubmissionDto
    {
        public long QuestionId { get; set; }
        public List<int> SelectedOptionIndexes { get; set; } = new();
    }

    public class QuizSubmissionDto
    {
        public long ChallengeId { get; set; }
        public List<QuestionAnswerSubmissionDto> Answers { get; set; } = new();
    }

    public class QuizResultDto
    {
        public int CorrectAnswersCount { get; set; }
        public int TotalQuestionsCount { get; set; }
        public bool IsSuccessful { get; set; }
        public int XpAwarded { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new();
    }

    public class QuestionResultDto
    {
        public long QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCompletelyCorrect { get; set; }
        public List<OptionResultDto> Options { get; set; } = new();
    }

    public class OptionResultDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public bool WasSelected { get; set; }
    }
}
