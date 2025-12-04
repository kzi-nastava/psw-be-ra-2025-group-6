using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases.Administration
{
    public class BlogVoteService : IBlogVoteService
    {
        private readonly IBlogVoteRepository _voteRepository;
        private readonly IMapper _mapper;

        public BlogVoteService(IBlogVoteRepository voteRepository, IMapper mapper)
        {
            _voteRepository = voteRepository;
            _mapper = mapper;
        }

        public void Vote(long userId, long blogPostId, VoteTypeDto typeDto)
        {
            var type = typeDto == VoteTypeDto.Upvote ? VoteType.Upvote : VoteType.Downvote;
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

        public BlogVoteDto? GetUserVote(long userId, long blogPostId)
        {
            var vote = _voteRepository.GetByUserAndBlog(userId, blogPostId);
            if (vote == null) return null;
            return _mapper.Map<BlogVoteDto>(vote);
        }
    }
}
