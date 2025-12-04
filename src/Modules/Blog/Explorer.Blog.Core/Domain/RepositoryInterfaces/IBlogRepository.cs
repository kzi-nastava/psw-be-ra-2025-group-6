using Explorer.BuildingBlocks.Core.UseCases;
using DomainBlog = Explorer.Blog.Core.Domain.BlogPost;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IBlogRepository
{
    PagedResult<BlogPost> GetPaged(int page, int pageSize);
    List<BlogPost> GetByUser(long id);
    BlogPost Create(BlogPost map);
    BlogPost Update(BlogPost map);
    BlogPost GetById(long id);
    void Delete(BlogPost blog);
}
