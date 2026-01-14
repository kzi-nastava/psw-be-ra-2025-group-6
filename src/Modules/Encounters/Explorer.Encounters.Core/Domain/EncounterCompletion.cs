using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterCompletion : Entity
    {
        public long UserId { get; private set; }
        public long ChallengeId { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public int XpAwarded { get; private set; }

        private EncounterCompletion() { }

        public EncounterCompletion(long userId, long challengeId, int xpAwarded)
        {
            if (userId <= 0) throw new ArgumentException("Invalid UserId.");
            if (challengeId <= 0) throw new ArgumentException("Invalid ChallengeId.");
            if (xpAwarded < 0) throw new ArgumentException("XP awarded cannot be negative.");

            UserId = userId;
            ChallengeId = challengeId;
            CompletedAt = DateTime.UtcNow;
            XpAwarded = xpAwarded;
        }
    }
}
