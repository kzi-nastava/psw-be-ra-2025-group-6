using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases.Administration;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public BlogService(IBlogRepository blogRepository, IUserRepository userRepository, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _userRepository = userRepository;
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
        var blog = new BlogPost(
            userId,
            dto.Title,
            dto.Description,
            new List<string>() 
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

    public void AddComment(long blogId, long userId, string text)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found.");
        var user = _userRepository.GetById(userId);
        if (user == null) throw new Exception("User not found.");

        var authorName = user.Username;

        blog.AddComment(userId, authorName, text);
        _blogRepository.Update(blog);
    }

    public void EditComment(long blogId, int commentId, long userId, string text)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found.");

        blog.EditComment(commentId, userId, text);
        _blogRepository.Update(blog);
    }

    public void DeleteComment(long blogId, int commentId, long userId) 
    { 
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found");

        blog.DeleteComment(commentId, userId);
        _blogRepository.Update(blog);
    }

    public List<CommentDto> GetComments(long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found");

        var comments = blog.Comments
            .Select((c, index) => new CommentDto
            {
                Id = index,     
                UserId = c.UserId,
                AuthorName = c.AuthorName,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                LastUpdatedAt = c.LastUpdatedAt
            })
            .ToList();

        return comments;
    }

}
