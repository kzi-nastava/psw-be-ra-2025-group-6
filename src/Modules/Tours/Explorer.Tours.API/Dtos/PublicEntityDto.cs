namespace Explorer.Tours.API.Dtos;
    public class PublicEntityDto
    {
        public List<PublicKeyPointDto> KeyPoints { get; set; } = new();
        public List<PublicFacilityDto> Facilities { get; set; } = new();
    }

