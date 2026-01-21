namespace Explorer.Encounters.API.Dtos
{
    public class TouristXpProfileDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int CurrentXP { get; set; }
        public int Level { get; set; }
        public int XpProgressInCurrentLevel { get; set; }
        public int XpNeededForNextLevel { get; set; }
        public int XpRequiredForNextLevel { get; set; }
        public bool CanCreateEncounters { get; set; }
        public List<LevelUpRecordDto> LevelUpHistory { get; set; } = new();
    }

    public class LevelUpRecordDto
    {
        public int Level { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
