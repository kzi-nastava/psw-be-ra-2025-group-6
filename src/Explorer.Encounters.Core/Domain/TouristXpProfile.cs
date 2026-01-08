using Explorer.BuildingBlocks.Core.Domain;
using System.Text.Json;

namespace Explorer.Encounters.Core.Domain
{
    public class TouristXpProfile : AggregateRoot
    {
        public long UserId { get; private set; }
        public int CurrentXP { get; private set; }
        public int Level { get; private set; }
        
        // JSON serialized list of level-up timestamps
        public string? LevelUpHistory { get; private set; }

        private TouristXpProfile() { }

        public TouristXpProfile(long userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid UserId.");
            
            UserId = userId;
            CurrentXP = 0;
            Level = 1;
            LevelUpHistory = JsonSerializer.Serialize(new List<LevelUpRecord>
            {
                new LevelUpRecord { Level = 1, Timestamp = DateTime.UtcNow }
            });
        }

        public void AddXP(int xpAmount)
        {
            if (xpAmount < 0) throw new ArgumentException("XP amount cannot be negative.");
            
            CurrentXP += xpAmount;
            CheckForLevelUp();
        }

        private void CheckForLevelUp()
        {
            while (CurrentXP >= GetXPRequiredForNextLevel())
            {
                Level++;
                var history = GetLevelUpHistoryList();
                history.Add(new LevelUpRecord { Level = Level, Timestamp = DateTime.UtcNow });
                LevelUpHistory = JsonSerializer.Serialize(history);
            }
        }

        public int GetXPRequiredForNextLevel()
        {
            // Formula: Level 2 = 100 XP, Level 3 = 250 XP, Level 4 = 450 XP, etc.
            // XP needed = 50 * Level * (Level + 1)
            return 50 * Level * (Level + 1);
        }

        public int GetXPForCurrentLevel()
        {
            if (Level == 1) return 0;
            return 50 * (Level - 1) * Level;
        }

        public int GetXPProgressInCurrentLevel()
        {
            return CurrentXP - GetXPForCurrentLevel();
        }

        public int GetXPNeededForNextLevel()
        {
            return GetXPRequiredForNextLevel() - CurrentXP;
        }

        public bool CanCreateEncounters()
        {
            return Level >= 10;
        }

        private List<LevelUpRecord> GetLevelUpHistoryList()
        {
            if (string.IsNullOrWhiteSpace(LevelUpHistory))
                return new List<LevelUpRecord>();
            
            return JsonSerializer.Deserialize<List<LevelUpRecord>>(LevelUpHistory) ?? new List<LevelUpRecord>();
        }

        public List<LevelUpRecord> GetLevelUpHistory()
        {
            return GetLevelUpHistoryList();
        }
    }

    public class LevelUpRecord
    {
        public int Level { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
