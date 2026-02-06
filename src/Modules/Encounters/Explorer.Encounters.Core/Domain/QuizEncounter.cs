using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class QuizEncounter : Entity
    {
        public long ChallengeId { get; private set; }
        public int MinimumCorrectAnswers { get; private set; }
        public string? AudioStoryPath { get; private set; }
        public ICollection<QuizQuestion> Questions { get; private set; }

        private QuizEncounter()
        {
            Questions = new List<QuizQuestion>();
        }

        public QuizEncounter(long challengeId, int minimumCorrectAnswers, string? audioStoryPath = null)
        {
            if (challengeId == 0) throw new ArgumentException("Invalid ChallengeId.");
            if (minimumCorrectAnswers <= 0) throw new ArgumentException("MinimumCorrectAnswers must be positive.");

            ChallengeId = challengeId;
            MinimumCorrectAnswers = minimumCorrectAnswers;
            AudioStoryPath = audioStoryPath;
            Questions = new List<QuizQuestion>();
        }

        public void AddQuestion(QuizQuestion question)
        {
            if (question == null) throw new ArgumentNullException(nameof(question));
            Questions.Add(question);
        }

        public void UpdateMinimumCorrectAnswers(int minimumCorrectAnswers)
        {
            if (minimumCorrectAnswers <= 0) throw new ArgumentException("MinimumCorrectAnswers must be positive.");
            if (minimumCorrectAnswers > Questions.Count) throw new ArgumentException("MinimumCorrectAnswers cannot exceed total questions count.");
            
            MinimumCorrectAnswers = minimumCorrectAnswers;
        }

        public void UpdateAudioStoryPath(string? audioStoryPath)
        {
            AudioStoryPath = audioStoryPath;
        }

        public bool IsPassingScore(int correctAnswersCount)
        {
            return correctAnswersCount >= MinimumCorrectAnswers;
        }
    }
}
