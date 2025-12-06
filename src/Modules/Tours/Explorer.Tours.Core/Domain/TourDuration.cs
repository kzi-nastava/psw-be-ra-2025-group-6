using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.Core.Domain;
using System.Text.Json.Serialization;

public class TourDuration : ValueObject
{
    public TravelType TravelType { get; private set; }
    public double Minutes { get; private set; }

    
    private TourDuration() { }

    [JsonConstructor]
    public TourDuration(TravelType travelType, double minutes)
    {
        if (minutes < 0) throw new ArgumentException("Invalid duration.");
        TravelType = travelType;
        Minutes = minutes;
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TravelType;
        yield return Minutes;
    }
    public void UpdateMinutes(double minutes)
    {
        if (minutes < 0)
            throw new ArgumentException("Invalid duration.");

        Minutes = minutes;
    }
}

