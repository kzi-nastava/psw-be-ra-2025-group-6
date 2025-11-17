using Explorer.Blog.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IBlogRepository
{
    PagedResult<Blog> GetPaged(int page, int pageSize);
    Blog GetById(long id);
    Blog Create(Blog map);
    Blog Update(Blog map);
}
