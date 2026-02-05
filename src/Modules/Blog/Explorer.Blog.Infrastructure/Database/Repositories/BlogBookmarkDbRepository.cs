using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class BlogBookmarkDbRepository : IBlogBookmarkRepository
    {
        private readonly BlogContext _dbContext;

        public BlogBookmarkDbRepository(BlogContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BlogBookmark Create(BlogBookmark bookmark)
        {
            _dbContext.Set<BlogBookmark>().Add(bookmark);
            _dbContext.SaveChanges();
            return bookmark;
        }

        public void Delete(long userId, long blogPostId)
        {
            var bookmark = _dbContext.Set<BlogBookmark>()
                .FirstOrDefault(b => b.UserId == userId && b.BlogPostId == blogPostId);
            if (bookmark != null)
            {
                _dbContext.Set<BlogBookmark>().Remove(bookmark);
                _dbContext.SaveChanges();
            }
        }

        public List<long> GetSavedPostIdsByUser(long userId)
        {
            return _dbContext.Set<BlogBookmark>()
                .Where(b => b.UserId == userId)
                .Select(b => b.BlogPostId)
                .ToList();
        }

        public bool IsBookmarked(long userId, long blogPostId)
        {
            return _dbContext.Set<BlogBookmark>()
                .Any(b => b.UserId == userId && b.BlogPostId == blogPostId);
        }
    }
}
