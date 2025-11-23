
namespace Explorer.Tours.API.Dtos
{
    public class MonumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? YearOfOrigin { get; set; }
        public string Status { get; set; } = "active";
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
    }
}
