using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class QuizQuestion : Entity
    {
        public string Text { get; private set; }
        public List<QuizAnswerOption> AnswerOptions { get; private set; }
        public long QuizEncounterId { get; private set; }

        private QuizQuestion()
        {
            Text = string.Empty;
            AnswerOptions = new List<QuizAnswerOption>();
        }

        public QuizQuestion(string text, List<QuizAnswerOption> answerOptions, long quizEncounterId)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Question text cannot be empty.");
            if (answerOptions == null || answerOptions.Count < 2)
                throw new ArgumentException("Must have at least 2 answer options.");
            if (!answerOptions.Any(o => o.IsCorrect))
                throw new ArgumentException("At least one answer option must be correct.");
            if (quizEncounterId == 0)
                throw new ArgumentException("Invalid QuizEncounterId.");

            Text = text;
            AnswerOptions = answerOptions;
            QuizEncounterId = quizEncounterId;
        }

        public bool IsCorrectAnswer(List<int> selectedIndexes)
        {
            if (selectedIndexes == null || !selectedIndexes.Any())
                return false;

            var correctIndexes = AnswerOptions
                .Select((option, index) => new { option, index })
                .Where(x => x.option.IsCorrect)
                .Select(x => x.index)
                .OrderBy(x => x)
                .ToList();

            var sortedSelected = selectedIndexes.OrderBy(x => x).ToList();

            return correctIndexes.SequenceEqual(sortedSelected);
        }

        public int GetCorrectAnswerCount()
        {
            return AnswerOptions.Count(o => o.IsCorrect);
        }
    }
}
