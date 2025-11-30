using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class TouristPosition : Entity
{
    public long TouristId { get; init; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public TouristPosition(long touristId, double latitude, double longitude)
    {
        TouristId = touristId;
        Latitude = latitude;
        Longitude = longitude;
        Validate();
    }

    public void Update(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Validate();
    }

    private void Validate()
    {
        if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
        if (Latitude < -90 || Latitude > 90) throw new ArgumentException("Invalid Latitude");
        if (Longitude < -180 || Longitude > 180) throw new ArgumentException("Invalid Longitude");
    }
}
