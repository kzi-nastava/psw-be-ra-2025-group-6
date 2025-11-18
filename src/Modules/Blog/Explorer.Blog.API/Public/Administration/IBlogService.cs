using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public.Administration;

public interface IBlogService
{
    PagedResult<BlogDto> GetPaged(int page, int pageSize);
    List<BlogDto> GetByUser(long id);                               
    BlogDto Create(BlogCreateDto blog, long id);                      
    BlogDto Update(BlogDto blog);
    BlogDto GetById(long id);
    BlogDto Delete(long id);
}
