using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class CompletedKeyPoint : ValueObject
    {
        public long KeyPointId { get; private set; }
        public string KeyPointName { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public string UnlockedSecret { get; private set; }

        private CompletedKeyPoint() { }

        public CompletedKeyPoint(long keyPointId, string keyPointName, string unlockedSecret)
        {
            if (keyPointId == 0) throw new ArgumentException("Invalid KeyPointId");
            if (string.IsNullOrWhiteSpace(keyPointName)) throw new ArgumentException("Invalid KeyPointName");
            if (string.IsNullOrWhiteSpace(unlockedSecret)) throw new ArgumentException("Invalid UnlockedSecret");

            KeyPointId = keyPointId;
            KeyPointName = keyPointName;
            UnlockedSecret = unlockedSecret;
            CompletedAt = DateTime.UtcNow;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return KeyPointId;
            yield return KeyPointName;
            yield return CompletedAt;
            yield return UnlockedSecret;
        }
    }
}
