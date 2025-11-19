namespace Explorer.Tours.API.Dtos
{
    public enum FacilityType { Toilet, Restaurant, Parking, Other }
    public class FacilityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public FacilityType Type { get; set; }
    }
}
