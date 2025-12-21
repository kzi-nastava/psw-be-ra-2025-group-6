namespace Explorer.Payments.API.Internal;

public interface ITourDataProvider
{
    TourData GetTourData(long tourId);
}

public class TourData
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Status { get; set; }
}
