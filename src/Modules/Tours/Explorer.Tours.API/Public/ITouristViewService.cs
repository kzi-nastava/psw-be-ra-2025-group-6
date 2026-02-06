namespace Explorer.Tours.API.Public
{
     public interface ITouristViewService
    {
        List<TouristTourDto> GetPublishedTours();
        void BookmarkTour(long touristId, long tourId);
        void RemoveBookmark(long touristId, long tourId);
        List<TouristTourDto> GetSavedTours(long touristId);

    }
}
