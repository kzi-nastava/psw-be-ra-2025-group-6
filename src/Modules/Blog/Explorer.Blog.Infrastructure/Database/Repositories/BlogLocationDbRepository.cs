using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class BlogLocationDbRepository : IBlogLocationRepository
    {
        protected readonly BlogContext DbContext;
        private readonly DbSet<BlogLocation> _dbSet;

        public BlogLocationDbRepository(BlogContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<BlogLocation>();
        }

        public BlogLocation Create(BlogLocation location)
        {
            _dbSet.Add(location);
            DbContext.SaveChanges();
            return location;
        }

        public BlogLocation? GetById(long id)
        {
            return _dbSet.FirstOrDefault(l => l.Id == id);
        }

        public IEnumerable<BlogLocation> GetAll()
        {
            return _dbSet.ToList();
        }

        public BlogLocation? FindByCoordinates(double lat, double lon, double tolerance)
        {
            return _dbSet.FirstOrDefault(l =>
                Math.Abs(l.Latitude - lat) <= tolerance &&
                Math.Abs(l.Longitude - lon) <= tolerance);
        }
    }
}
