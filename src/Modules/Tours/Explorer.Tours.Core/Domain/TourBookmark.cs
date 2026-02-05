using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourBookmark : Entity
    {
        public long TouristId { get; init; }
        public long TourId { get; init; }
        public DateTime SavedAt { get; init; }

        public TourBookmark(long touristId, long tourId)
        {
            TouristId = touristId;
            TourId = tourId;
            SavedAt = DateTime.UtcNow;
        }
    }
}
