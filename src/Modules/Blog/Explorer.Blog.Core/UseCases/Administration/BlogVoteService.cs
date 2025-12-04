using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.UseCases.Administration
{
    public class BlogVoteService
    {
        private readonly IBlogVoteRepository _voteRepository;

        public BlogVoteService(IBlogVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public void Vote(long userId, long blogPostId, VoteType type)
        {
            var existingVote = _voteRepository.GetByUserAndBlog(userId, blogPostId);

            if (existingVote == null)
            {
                var vote = new BlogVote(blogPostId, userId, type);
                _voteRepository.Create(vote);
            }
            else if (existingVote.Type == type)
            {
                _voteRepository.Delete(existingVote);
            }
            else
            {
                existingVote.UpdateVote(type);
                _voteRepository.Update(existingVote);
            }
        }

        public (int upvotes, int downvotes) GetVotes(long blogPostId)
        {
            var up = _voteRepository.CountUpvotes(blogPostId);
            var down = _voteRepository.CountDownvotes(blogPostId);
            return (up, down);
        }

        public BlogVote GetUserVote(long userId, long blogPostId)
        {
            return _voteRepository.GetByUserAndBlog(userId, blogPostId);
        }
    }
}
