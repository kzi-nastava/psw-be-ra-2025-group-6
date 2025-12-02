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

    public PagedResult<BlogDto> GetPaged(int page, int pageSize)
    {
        var result = _blogRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<BlogDto>).ToList();
        return new PagedResult<BlogDto>(items, result.TotalCount);
    }

    public List<BlogDto> GetByUser(long userId)
    {
        var blogs = _blogRepository.GetByUser(userId);
        return blogs.Select(blog => _mapper.Map<BlogDto>(blog)).ToList();
    }

    public BlogDto Create(BlogCreateDto dto, long userId)
    {
        var status = _mapper.Map<BlogStatus>(dto.Status);
        var blog = new BlogPost(
            userId,
            dto.Title,
            dto.Description,
            new List<string>(),
            status
        );
        var created = _blogRepository.Create(blog);
        return _mapper.Map<BlogDto>(created);
    }

    public BlogDto Update(BlogDto blogDto)
    {
        var blog = _mapper.Map<BlogPost>(blogDto);
        var updated = _blogRepository.Update(blog);
        return _mapper.Map<BlogDto>(updated);
    }

    public BlogDto GetById(long id)
    {
        var blog = _blogRepository.GetById(id);
        if (blog == null) return null;

        return _mapper.Map<BlogDto>(blog);
    }

    public BlogDto Delete(long id)
    {
        var blog = _blogRepository.GetById(id);
        if (blog == null) return null;

        _blogRepository.Delete(blog);
        return _mapper.Map<BlogDto>(blog);
    }
    public void AddImages(long blogId, List<string> imagePaths)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");

        blog.AddImages(imagePaths);
        _blogRepository.Update(blog);
    }
}
