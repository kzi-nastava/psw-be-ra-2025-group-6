namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ImageUris { get; set; } = new List<string>();
        public long OwnerId { get; set; }
    }
}
