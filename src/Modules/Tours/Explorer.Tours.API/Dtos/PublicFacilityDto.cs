namespace Explorer.Tours.API.Dtos;
    public class PublicFacilityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public FacilityType Type { get; set; }
}

