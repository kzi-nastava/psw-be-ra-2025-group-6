namespace Explorer.Tours.API.Dtos;

public class TourExecutionStartDto
{
    public long TourId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class TourExecutionStartResultDto
{
    public long TourExecutionId { get; set; }
    public long TourId { get; set; }
    public long TouristId { get; set; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public TrackPointDto InitialPosition { get; set; }

    public KeyPointDto? FirstKeyPoint { get; set; }
    public List<TrackPointDto>? RouteToFirstKeyPoint { get; set; }
}

public class TourExecutionResultDto
{
    public long TourExecutionId { get; set; }
    public long TourId { get; set; }
    public long TouristId { get; set; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime LastActivity { get; set; }
}

public class TrackPointDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
