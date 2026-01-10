using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;
public class CommentLike : Entity
{
    public long BlogId { get; private set; }
    public long CommentId { get; private set; }
    public long UserId { get; private set; }

    public CommentLike() { }

    public CommentLike(long blogId, long commentId, long userId)
    {
        BlogId = blogId;
        CommentId = commentId;
        UserId = userId;
    }
}

