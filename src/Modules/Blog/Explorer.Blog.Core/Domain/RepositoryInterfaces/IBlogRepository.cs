using Explorer.BuildingBlocks.Core.UseCases;
using DomainBlog = Explorer.Blog.Core.Domain.Blog;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IBlogRepository
{
    PagedResult<Blog> GetPaged(int page, int pageSize);
    List<Blog> GetByUser(long id);
    Blog Create(Blog map);
    Blog Update(Blog map);
    Blog GetById(long id);
    void Delete(DomainBlog blog);
}
