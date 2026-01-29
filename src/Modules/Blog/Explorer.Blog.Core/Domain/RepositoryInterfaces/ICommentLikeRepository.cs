namespace Explorer.Blog.Core.Domain.RepositoryInterfaces; 
public interface ICommentLikeRepository
{
    bool Toggle(long blogId, long commentId, long userId);
    int CountLikes(long blogId, long commentId);
    bool IsLikedByUser(long blogId, long commentId, long userId);
}

