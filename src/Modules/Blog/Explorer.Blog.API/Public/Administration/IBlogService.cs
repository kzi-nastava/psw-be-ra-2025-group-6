using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public.Administration;

public interface IBlogService
{
    PagedResult<BlogDto> GetBlogs(int page, int pageSize);  
    BlogDto GetById(long id);                               
    BlogDto CreateBlog(BlogDto blog);                      
    BlogDto UpdateBlog(BlogDto blog);
}
