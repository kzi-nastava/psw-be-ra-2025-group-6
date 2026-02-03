namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMemberDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTime JoinedAt { get; set; }
        public string Status { get; set; }
    }

    public class InviteToClubDto
    {
        public string Username { get; set; }
    }
}
