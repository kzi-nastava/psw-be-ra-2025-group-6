using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class QuizAnswerOption : ValueObject
    {
        public string Text { get; private set; }
        public bool IsCorrect { get; private set; }

        private QuizAnswerOption()
        {
            Text = string.Empty;
        }

        public QuizAnswerOption(string text, bool isCorrect)
        {
            if (string.IsNullOrWhiteSpace(text)) 
                throw new ArgumentException("Answer option text cannot be empty.");

            Text = text;
            IsCorrect = isCorrect;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Text;
            yield return IsCorrect;
        }
    }
}
