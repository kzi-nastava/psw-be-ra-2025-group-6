using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class BlogVoteDbRepository : IBlogVoteRepository
    {
        private readonly BlogContext _dbContext;
        private readonly DbSet<BlogVote> _dbSet;

        public BlogVoteDbRepository(BlogContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<BlogVote>();
        }

        public BlogVote GetByUserAndBlog(long userId, long blogPostId)
        {
            return _dbSet.FirstOrDefault(v => v.UserId == userId && v.BlogPostId == blogPostId);
        }

        public void Create(BlogVote vote)
        {
            _dbSet.Add(vote);
            _dbContext.SaveChanges();
        }

        public void Update(BlogVote vote)
        {
            _dbContext.Entry(vote).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void Delete(BlogVote vote)
        {
            _dbSet.Remove(vote);
            _dbContext.SaveChanges();
        }

        public int CountUpvotes(long blogPostId)
        {
            return _dbSet.Count(v => v.BlogPostId == blogPostId && v.Type == VoteType.Upvote);
        }

        public int CountDownvotes(long blogPostId)
        {
            return _dbSet.Count(v => v.BlogPostId == blogPostId && v.Type == VoteType.Downvote);
        }
    }
}
