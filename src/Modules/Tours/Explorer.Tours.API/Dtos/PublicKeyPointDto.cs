namespace Explorer.Tours.API.Dtos;
    public class PublicKeyPointDto
    {
        public long Id;
        public long TourId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ImagePath { get; set; }
        public bool IsPublic { get; set; }
        public long? PublicRequestId { get; set; }
}

