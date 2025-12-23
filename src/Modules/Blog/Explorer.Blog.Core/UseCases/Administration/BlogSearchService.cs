using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;
using Shared;
using System.Security.Claims;
namespace Explorer.Blog.Core.UseCases.Administration;

public class BlogSearchService : IBlogSearchService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;
    private readonly IInternalStakeholderService _stakeholderService;
    public BlogSearchService(IBlogRepository blogRepository, IInternalStakeholderService stakeholderService, IMapper mapper)
    {
        _blogRepository = blogRepository;
        _stakeholderService = stakeholderService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
        string query,
        ClaimsPrincipal user, long personId)
    {
        var isAuthor = user.IsInRole("Author");
        var isAdmin = user.IsInRole("Admin");

        return _blogRepository.GetAll()
            .Where(b =>
                b.Title.Contains(query) &&
                (
                    b.Status == BlogStatus.POSTED ||
                    isAdmin ||
                    (isAuthor && b.UserId == personId)
                )
            )
            .Select(b => new SearchItemDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                Type = SearchEntityType.Blog,
                Url = $"/blogs/{b.Id}"
            })
            .ToList();
    }
}
