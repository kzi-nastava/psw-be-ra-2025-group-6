namespace Explorer.Tours.API.Dtos
{
    public class JournalDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public DateTime TravelDate { get; set; }
        public string Status { get; set; }
    }
}

