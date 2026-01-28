using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Shared;
using Shared.Achievements;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases;

public class ProfilePostService : IProfilePostService
{
    private readonly IProfilePostRepository _repository;
    private readonly ITourInfoGateway _tourInfoGateway;
    private readonly IBlogInfoGateway _blogInfoGateway;
    private readonly IMapper _mapper;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public ProfilePostService(IProfilePostRepository repository, ITourInfoGateway tourInfoGateway, IBlogInfoGateway blogInfoGateway, IMapper mapper, IDomainEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _tourInfoGateway = tourInfoGateway;
        _blogInfoGateway = blogInfoGateway;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
    }

    public ProfilePostDto Create(ProfilePostDto dto)
    {
        ValidateResource(dto).GetAwaiter().GetResult();

        HandleProfilePostAchievements(dto.AuthorId);

        var entity = new ProfilePost(
            dto.AuthorId,
            dto.Text,
            MapResourceType(dto.ResourceType),
            dto.ResourceId
        );

        var created = _repository.Create(entity);
        return _mapper.Map<ProfilePostDto>(created);
    }


    public ProfilePostDto Update(ProfilePostDto dto)
    {
        var existing = _repository.Get(dto.Id);
        if (existing.AuthorId != dto.AuthorId)
            throw new ForbiddenException("You can only edit your own profile posts.");

        ValidateResource(dto).GetAwaiter().GetResult();
        existing.Update(dto.Text, MapResourceType(dto.ResourceType), dto.ResourceId);
        var updated = _repository.Update(existing);
        return _mapper.Map<ProfilePostDto>(updated);
    }

    public void Delete(long id, long authorId)
    {
        var existing = _repository.Get(id);
        if (existing.AuthorId != authorId)
            throw new ForbiddenException("You can only delete your own profile posts.");
        _repository.Delete(id);
    }

    public List<ProfilePostDto> GetByAuthor(long authorId)
    {
        return _repository.GetByAuthor(authorId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(_mapper.Map<ProfilePostDto>)
            .ToList();
    }

    public PagedResult<ProfilePostDto> GetPagedByAuthor(long authorId, int page, int pageSize)
    {
        var posts = GetByAuthor(authorId);
        var total = posts.Count;
        var items = posts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PagedResult<ProfilePostDto>(items, total);
    }

    private async Task ValidateResource(ProfilePostDto dto)
    {
        if (dto.ResourceType == null)
        {
            return;
        }

        if (!dto.ResourceId.HasValue || dto.ResourceId.Value <= 0)
            throw new ArgumentException("Invalid ResourceId");

        if (dto.ResourceType == ProfileResourceTypeDto.Tour)
        {
            var tour = await _tourInfoGateway.GetById(dto.ResourceId.Value);
            if (tour == null)
                throw new ArgumentException("Referenced tour does not exist.");
        }
        else if (dto.ResourceType == ProfileResourceTypeDto.Blog)
        {
            var blog = await _blogInfoGateway.GetById(dto.ResourceId.Value);
            if (blog == null)
                throw new ArgumentException("Referenced blog does not exist.");
        }
    }

    private static ProfileResourceType? MapResourceType(ProfileResourceTypeDto? dto)
    {
        return dto.HasValue ? (ProfileResourceType?)dto.Value : null;
    }

    private void HandleProfilePostAchievements(long authorId)
    {
        var postCount = GetByAuthor(authorId).Count;

        if (postCount == 0)
        {
            _eventDispatcher
                .DispatchAsync(new AchievementUnlockedEvent(authorId, 17))
                .GetAwaiter().GetResult();
        }
        else if (postCount == 9)
        {
            _eventDispatcher
                .DispatchAsync(new AchievementUnlockedEvent(authorId, 18))
                .GetAwaiter().GetResult();
        }
    }

}
