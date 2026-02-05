using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    internal class TourBookmarkDbRepository : ITourBookmarkRepository
    {
        private readonly ToursContext _dbContext;

        public TourBookmarkDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TourBookmark Create(TourBookmark bookmark)
        {
            _dbContext.TourBookmarks.Add(bookmark);
            _dbContext.SaveChanges();
            return bookmark;
        }

        public void Delete(long touristId, long tourId)
        {
            var bookmark = _dbContext.TourBookmarks
                .FirstOrDefault(tb => tb.TouristId == touristId && tb.TourId == tourId);

            if (bookmark != null)
            {
                _dbContext.TourBookmarks.Remove(bookmark);
                _dbContext.SaveChanges();
            }
        }

        public bool IsBookmarked(long touristId, long tourId)
        {
            return _dbContext.TourBookmarks
                .Any(tb => tb.TouristId == touristId && tb.TourId == tourId);
        }

        public List<Tour> GetSavedTours(long touristId)
        {
            return _dbContext.TourBookmarks
                .Where(tb => tb.TouristId == touristId)
                .Join(_dbContext.Tours,
                    bookmark => bookmark.TourId,
                    tour => tour.Id,
                    (bookmark, tour) => tour)
                .Include(t => t.KeyPoints)
                .ToList();
        }

    }
}
