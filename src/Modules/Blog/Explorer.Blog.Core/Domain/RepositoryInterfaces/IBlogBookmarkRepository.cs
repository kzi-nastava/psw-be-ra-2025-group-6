
namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogBookmarkRepository
    {
        BlogBookmark Create(BlogBookmark bookmark);
        void Delete(long userId, long blogPostId);
        List<long> GetSavedPostIdsByUser(long userId);
        bool IsBookmarked(long userId, long blogPostId);
    }
}
