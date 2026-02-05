namespace Explorer.Encounters.API.Dtos
{
    public class ChallengeDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int XP { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public long? CreatorId { get; set; }
        public bool IsCreatedByTourist { get; set; }
        public string? ImagePath { get; set; }
        public int ActivationRadiusMeters { get; set; }
        public long? KeyPointId { get; set; }
        public bool IsRequiredForSecret { get; set; }

        // For Social Encounter type
        public int? RequiredPeople { get; set; }
        public double? SocialRadiusMeters { get; set; }
    }
}
