namespace Explorer.Payments.API.Internal;

public interface ITourDataProvider
{
    TourData GetTourData(long tourId);
    bool VerifyToursOwnership(long authorId, List<long> tourIds);
    int GetPublishedToursCount(List<long> tourIds);
    double GetTotalPrice(List<long> tourIds);
    SaleInfo? GetActiveSaleForTour(long tourId);
}

public class TourData
{
    public long Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Status { get; set; }
}

public class SaleInfo
{
    public int DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
