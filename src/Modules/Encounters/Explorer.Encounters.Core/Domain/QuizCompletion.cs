using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class QuizCompletion : Entity
    {
        public long UserId { get; private set; }
        public long QuizEncounterId { get; private set; }
        public long ChallengeId { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public int CorrectAnswersCount { get; private set; }
        public int TotalQuestionsCount { get; private set; }
        public bool IsSuccessful { get; private set; }
        public int XpAwarded { get; private set; }

        private QuizCompletion() { }

        public QuizCompletion(long userId, long quizEncounterId, long challengeId, int correctAnswersCount, int totalQuestionsCount, bool isSuccessful, int xpAwarded)
        {
            if (userId == 0) throw new ArgumentException("Invalid UserId.");
            if (quizEncounterId == 0) throw new ArgumentException("Invalid QuizEncounterId.");
            if (challengeId == 0) throw new ArgumentException("Invalid ChallengeId.");
            if (correctAnswersCount < 0) throw new ArgumentException("CorrectAnswersCount cannot be negative.");
            if (totalQuestionsCount <= 0) throw new ArgumentException("TotalQuestionsCount must be positive.");
            if (correctAnswersCount > totalQuestionsCount) throw new ArgumentException("CorrectAnswersCount cannot exceed TotalQuestionsCount.");
            if (xpAwarded < 0) throw new ArgumentException("XpAwarded cannot be negative.");

            UserId = userId;
            QuizEncounterId = quizEncounterId;
            ChallengeId = challengeId;
            CompletedAt = DateTime.UtcNow;
            CorrectAnswersCount = correctAnswersCount;
            TotalQuestionsCount = totalQuestionsCount;
            IsSuccessful = isSuccessful;
            XpAwarded = xpAwarded;
        }
    }
}
