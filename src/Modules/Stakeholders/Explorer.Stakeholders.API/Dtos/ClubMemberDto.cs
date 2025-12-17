namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMemberDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ClubId { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
