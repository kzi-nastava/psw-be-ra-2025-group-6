using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogVoteRepository
    {
        BlogVote GetByUserAndBlog(long userId, long blogPostId);
        void Create(BlogVote vote);
        void Update(BlogVote vote);
        void Delete(BlogVote vote);
        int CountUpvotes(long blogPostId);
        int CountDownvotes(long blogPostId);
    }
}
