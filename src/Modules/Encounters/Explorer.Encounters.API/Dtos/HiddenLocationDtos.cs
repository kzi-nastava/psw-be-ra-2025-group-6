namespace Explorer.Encounters.API.Dtos
{
    public class StartHiddenLocationDto
    {
        public long ChallengeId { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
    }

    public class UpdateHiddenLocationProgressDto
    {
        public long AttemptId { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
    }

    public class HiddenLocationAttemptDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ChallengeId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsSuccessful { get; set; }
        public int SecondsInRadius { get; set; }
        public DateTime LastPositionUpdate { get; set; }
    }

    public class HiddenLocationProgressDto
    {
        public long AttemptId { get; set; }
        public int SecondsInRadius { get; set; }
        public bool IsSuccessful { get; set; }
        public bool IsInRadius { get; set; }
        public double DistanceToTarget { get; set; }
        
        // XP and leveling information
        public int? XpAwarded { get; set; }
        public bool LeveledUp { get; set; }
        public int? NewLevel { get; set; }
    }

    public class ActivationCheckDto
    {
        public bool CanActivate { get; set; }
        public double DistanceToTarget { get; set; }
        public int ActivationRadius { get; set; }
        public string Message { get; set; }
    }
}
