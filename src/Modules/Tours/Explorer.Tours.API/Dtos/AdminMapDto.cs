namespace Explorer.Tours.API.Dtos
{
    public class AdminMapDto
    {
        public string Id { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
