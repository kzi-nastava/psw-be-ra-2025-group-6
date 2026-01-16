using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class HiddenLocationAttempt : Entity
    {
        public long UserId { get; private set; }
        public long ChallengeId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public bool IsSuccessful { get; private set; }
        public int SecondsInRadius { get; private set; }
        public DateTime LastPositionUpdate { get; private set; }

        private HiddenLocationAttempt() { }

        public HiddenLocationAttempt(long userId, long challengeId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid UserId.");
            if (challengeId <= 0) throw new ArgumentException("Invalid ChallengeId.");

            UserId = userId;
            ChallengeId = challengeId;
            StartedAt = DateTime.UtcNow;
            LastPositionUpdate = DateTime.UtcNow;
            SecondsInRadius = 0;
            IsSuccessful = false;
        }

        public void UpdateProgress(bool isInRadius)
        {
            var now = DateTime.UtcNow;
            var secondsElapsed = (int)(now - LastPositionUpdate).TotalSeconds;

            if (isInRadius)
            {
                SecondsInRadius += secondsElapsed;
            }
            else
            {
                SecondsInRadius = 0;
            }

            LastPositionUpdate = now;

            if (SecondsInRadius >= 30 && !IsSuccessful)
            {
                IsSuccessful = true;
            }
        }

        public bool CanComplete()
        {
            return IsSuccessful;
        }

        public void Complete()
        {
            if (!IsSuccessful)
                throw new InvalidOperationException("Cannot complete unsuccessful attempt.");

            CompletedAt = DateTime.UtcNow;
        }
    }
}
