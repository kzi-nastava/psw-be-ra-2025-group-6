namespace Explorer.Encounters.API.Dtos
{
    public class EncounterCompletionDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ChallengeId { get; set; }
        public DateTime CompletedAt { get; set; }
        public int XpAwarded { get; set; }
    }

    public class CompleteEncounterRequestDto
    {
        public long ChallengeId { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
    }

    public class CompleteEncounterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int XpAwarded { get; set; }
        public bool LeveledUp { get; set; }
        public int? NewLevel { get; set; }
        public TouristXpProfileDto Profile { get; set; }
    }

    public class ActiveChallengesResponseDto
    {
        public List<ChallengeDto> AvailableChallenges { get; set; } = new();
        public List<long> CompletedChallengeIds { get; set; } = new();
    }
}
