namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;
public interface ICommentReportRepository
{
    bool Exists(long blogId, long commentId, long userId);

    CommentReport Create(CommentReport report);

    CommentReport Get(long blogId, long commentId, long userId);

    CommentReport GetById(long id);

    IEnumerable<CommentReport> GetOpen(int skip, int take);

    int CountOpen();
}

