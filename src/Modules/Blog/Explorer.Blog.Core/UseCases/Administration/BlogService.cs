using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Blog.Core.UseCases.Administration;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;

    public BlogService(IBlogRepository blogRepository, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _mapper = mapper;
    }

    public PagedResult<BlogDto> GetBlogs(int page, int pageSize)
    {
        var result = _blogRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<BlogDto>).ToList();
        return new PagedResult<BlogDto>(items, result.TotalCount);
    }

    public BlogDto GetById(long id)
    {
        var blog = _blogRepository.GetById(id);
        return _mapper.Map<BlogDto>(blog);
    }

    public BlogDto CreateBlog(BlogDto blogDto)
    {
        var blog = _mapper.Map<Explorer.Blog.Core.Domain.Blog>(blogDto);
        var created = _blogRepository.Create(blog);
        return _mapper.Map<BlogDto>(created);
    }

    public BlogDto UpdateBlog(BlogDto blogDto)
    {
        var blog = _mapper.Map<Explorer.Blog.Core.Domain.Blog>(blogDto);
        var updated = _blogRepository.Update(blog);
        return _mapper.Map<BlogDto>(updated);
    }
}
