using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public.Administration
{
    public interface IBlogVoteService
    {
        void Vote(long userId, long blogPostId, VoteTypeDto type);
        (int upvotes, int downvotes) GetVotes(long blogPostId);
        BlogVoteDto? GetUserVote(long userId, long blogPostId);
    }
}
