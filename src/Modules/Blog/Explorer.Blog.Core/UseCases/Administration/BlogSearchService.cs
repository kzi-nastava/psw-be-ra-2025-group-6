using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Shared;
using System.Diagnostics;
using System.Security.Claims;
namespace Explorer.Blog.Core.UseCases.Administration;

public class BlogSearchService : IBlogSearchService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;
    public BlogSearchService(IBlogRepository blogRepository, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
        string query,
        ClaimsPrincipal user, long personId, string userRole)
    {

        var isAuthor = userRole== UserRole.Author.ToString();
        var isAdmin = userRole== UserRole.Administrator.ToString();

        var blogs = _blogRepository.GetAll()
            .Where(b =>
                b.Title.Contains(query) &&
                (
                    b.Status == BlogStatus.POSTED ||
                    isAdmin ||
                    (isAuthor && b.UserId == personId)
                )
            );
        var blogList = blogs
            .Select(b => new SearchItemDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Type = SearchEntityType.Blog,
                Url = $"/blogs/{b.Id}",
                Photo = b.Images.FirstOrDefault()
            })
            .ToList();
        foreach (var blog in blogs)
        {
            Debug.WriteLine($"Found blog: {blog.Title}");
            Debug.WriteLine($"Blog personId: {blog.UserId}");
            Debug.WriteLine($"Searching personId: {personId}");
            Debug.WriteLine($"isAdmin: {isAdmin}, isAuthor: {isAuthor}");
        }

        return blogList;
    }
}
