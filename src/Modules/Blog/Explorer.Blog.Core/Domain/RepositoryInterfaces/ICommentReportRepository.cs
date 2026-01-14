namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;
public interface ICommentReportRepository
{
    bool Exists(long blogId, long commentId, long userId);
    CommentReport Create(CommentReport report);
    CommentReport Get(long blogId, long commentId, long userId);
    CommentReport GetById(long id);
    IEnumerable<CommentReport> GetByReportStatus(AdminReportStatus status, int skip, int take);
    int CountByStatus(AdminReportStatus status);
    CommentReport Update(CommentReport report);
    void DeleteOpenByComment(long blogId, long commentId);
}

