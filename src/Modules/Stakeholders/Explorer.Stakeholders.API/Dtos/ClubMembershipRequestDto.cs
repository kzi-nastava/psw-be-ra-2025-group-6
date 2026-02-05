namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMembershipRequestDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public long TouristId { get; set; }
        public string? TouristUsername { get; set; } 
        public DateTime RequestedAt { get; set; }
        public int Status { get; set; } // 0-Processing, 1-Accepted, 2-Rejected
    }
}