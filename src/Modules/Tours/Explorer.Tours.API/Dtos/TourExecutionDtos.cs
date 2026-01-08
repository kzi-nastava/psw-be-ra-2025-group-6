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
    public double ProgressPercentage { get; set; }

}

public class RecentTourExecutionResultDto
{
    public long TourExecutionId { get; set; }
    public long TourId { get; set; }

    public string TourName { get; set; }
    public string TourDescription { get; set; }
    public long TouristId { get; set; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime LastActivity { get; set; }
    public double ProgressPercentage { get; set; }

    public KeyPointDto? FirstKeyPoint { get; set; }
}

public class TrackPointDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class ProgressResponseDto
{
    public bool KeyPointCompleted { get; set; }
    public CompletedKeyPointDto? CompletedKeyPoint { get; set; }
    public double ProgressPercentage { get; set; }
    public NextKeyPointDto? NextKeyPoint { get; set; }
    public DateTime LastActivity { get; set; }
    
    // Lista svih kompletiranih ta?aka (za prikaz ukupnog broja)
    public List<CompletedKeyPointDto> AllCompletedKeyPoints { get; set; } = new();
}

public class CompletedKeyPointDto
{
    public long KeyPointId { get; set; }
    public string KeyPointName { get; set; }
    public string UnlockedSecret { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class NextKeyPointDto
{
    public long KeyPointId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ImagePath { get; set; }
}

public class UnlockedSecretsDto
{
    public List<SecretDto> Secrets { get; set; } = new();
}

public class SecretDto
{
    public long KeyPointId { get; set; }
    public string KeyPointName { get; set; }
    public string Secret { get; set; }
    public DateTime UnlockedAt { get; set; }
}
