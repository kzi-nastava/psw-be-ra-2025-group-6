namespace Explorer.Tours.API.Dtos;

public class TourSearchByDistanceRequestDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    // Kilometers
    public double DistanceInKm { get; set; }
}
