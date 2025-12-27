using Explorer.Blog.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Blog.API.Public.Administration;

public interface IBlogService
{
    PagedResult<BlogDto> GetPaged(int page, int pageSize);
    List<BlogDto> GetByUser(long id);
    BlogDto Create(BlogCreateDto blog, long id);
    BlogDto Update(BlogDto blog);
    BlogDto GetById(long id);
    BlogDto Delete(long id);
    void AddImages(long blogId, List<string> imagePaths);
    CommentDto AddComment(long blogId, long userId, string text);
    CommentDto EditComment(long blogId, int commentId, long userId, string text);
    CommentDto DeleteComment(long blogId, int commentId, long userId);
    List<CommentDto> GetComments(long id);
    void Archive(long blogId);
    BlogDto UpdateDescription(long blogId, string newDescription);
    void Vote(long userId, long blogId, VoteTypeDto voteType);
    public void RemoveVote(long userId, long blogId);
    (int upvotes, int downvotes) GetVotes(long blogId);
    BlogVoteDto? GetUserVote(long userId, long blogId);
    BlogDto RecalculateQualityStatus(long blogId);
    List<BlogDto> GetBlogsByQualityStatus(BlogQualityStatusDto status);
    PagedResult<BlogDto> GetFollowingBlogs(int page, int pageSize, long userId);
}
