using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogBookmark : Entity
    {
        public long UserId { get; private set; }
        public long BlogPostId { get; private set; }
        public DateTime SavedAt { get; private set; }

        public BlogBookmark(long userId, long blogPostId)
        {
            UserId = userId;
            BlogPostId = blogPostId;
            SavedAt = DateTime.UtcNow;
        }
    }
}
