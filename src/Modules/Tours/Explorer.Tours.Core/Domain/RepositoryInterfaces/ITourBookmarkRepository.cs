namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourBookmarkRepository
    {
        TourBookmark Create(TourBookmark bookmark);
        void Delete(long touristId, long tourId);
        bool IsBookmarked(long touristId, long tourId);
        List<Tour> GetSavedTours(long touristId);
    }
}
