
namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogLocationRepository
    {
        BlogLocation Create(BlogLocation location);
        BlogLocation? GetById(long id);
        IEnumerable<BlogLocation> GetAll();
        BlogLocation? FindByCoordinates(double latitude, double longitude, double tolerance);
    }
}
