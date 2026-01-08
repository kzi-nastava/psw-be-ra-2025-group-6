using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;
using System.Text.Json;

namespace Explorer.Blog.Core.UseCases.Administration;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;
    private readonly IInternalStakeholderService _stakeholderService;
    private readonly IBlogLocationService _locationService;

    public BlogService(IBlogRepository blogRepository, IInternalStakeholderService stakeholderService, IMapper mapper, IBlogLocationService locationService)
    {
        _blogRepository = blogRepository;
        _stakeholderService = stakeholderService;
        _mapper = mapper;
        _locationService = locationService;
    }

    public PagedResult<BlogDto> GetPaged(int page, int pageSize)
    {
        var result = _blogRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(MapBlogWithUsername).ToList();
        return new PagedResult<BlogDto>(items, result.TotalCount);
    }

    public List<BlogDto> GetByUser(long userId)
    {
        var blogs = _blogRepository.GetByUser(userId);
        return blogs.Select(MapBlogWithUsername).ToList();
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

        if (!string.IsNullOrWhiteSpace(dto.City))
        {
            // 1. Kreiramo DTO za lokaciju od podataka iz bloga
            var locationDto = new BlogLocationDto
            {
                City = dto.City,
                Country = dto.Country,
                Region = dto.Region,
                Latitude = dto.Latitude ?? 0,
                Longitude = dto.Longitude ?? 0
            };

            // 2. SERVIS kreira lokaciju u svojoj tabeli i vraća nam je sa dodeljenim ID-jem iz baze
            var savedLocationDto = _locationService.CreateOrGet(locationDto);

            // 3. POSTAVLJAMO ID lokacije u blog. 
            // Ovo je ključno: BlogPost entitet sada zna tačan ID iz tabele lokacija.
            blog.SetLocationId(savedLocationDto.Id);
        }


        if (dto.ContentItems != null)
        {
            foreach (var item in dto.ContentItems.OrderBy(i => i.Order))
            {
                blog.AddContentItem((ContentType)item.Type, item.Content);
            }
        }

        var created = _blogRepository.Create(blog);
        return _mapper.Map<BlogDto>(created);
    }

    public BlogDto Update(BlogDto blogDto)
    {
        //var blog = _mapper.Map<BlogPost>(blogDto);
        var blog = _blogRepository.GetById(blogDto.Id);
        if (blog == null)
            throw new NotFoundException("Blog not found");

        if (blog.Status == BlogStatus.ARCHIVED)
            throw new Exception("Cannot edit archived blogs.");

        blog.UpdateTitle(blogDto.Title);
        blog.UpdateDescription(blogDto.Description);
        blog.Status = (BlogStatus)blogDto.Status;
        if (blogDto.Location != null)
        {
            var savedLocation = _locationService.CreateOrGet(blogDto.Location);
            blog.SetLocation(_mapper.Map<BlogLocation>(savedLocation));
        }

        blog.ClearContentItems();
        if (blogDto.ContentItems != null)
        {
            foreach (var item in blogDto.ContentItems.OrderBy(i => i.Order))
            {
                blog.AddContentItem(
                    (ContentType)item.Type,
                    item.Content
                );
            }
        }

        var updated = _blogRepository.Update(blog);
        return _mapper.Map<BlogDto>(updated);
    }

    public BlogDto GetById(long id)
    {
        var blog = _blogRepository.GetById(id);
        if (blog == null) return null;
        return MapBlogWithUsername(blog);
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

    public CommentDto AddComment(long blogId, long userId, string text)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found.");
        var authorName = _stakeholderService.GetUsername(userId);

        blog.AddComment(userId, authorName, text);
        _blogRepository.Update(blog);

        var comment = blog.Comments.Last();

        return _mapper.Map<CommentDto>(comment);
    }

    public CommentDto EditComment(long blogId, int commentId, long userId, string text)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found.");

        blog.EditComment(commentId, userId, text);
        _blogRepository.Update(blog);

        var comment = blog.Comments.Last();

        return _mapper.Map<CommentDto>(comment);
    }

    public CommentDto DeleteComment(long blogId, int commentId, long userId) 
    { 
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found");

        blog.DeleteComment(commentId, userId);
        _blogRepository.Update(blog);

        var comment = blog.Comments.Last();

        return _mapper.Map<CommentDto>(comment);
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
    public void Archive(long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");

        if (blog.Status != BlogStatus.POSTED)
            throw new Exception("Only posted blogs can be archived.");

        blog.Status = BlogStatus.ARCHIVED;

        _blogRepository.Update(blog);
    }

    public BlogDto UpdateDescription(long blogId, string newDescription)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");

        if (blog.Status != BlogStatus.POSTED)
            throw new Exception("Only posted blogs can update description with this endpoint.");

        blog.UpdateDescription(newDescription);
        var updated = _blogRepository.Update(blog);
        return _mapper.Map<BlogDto>(updated);
    }

    public void Vote(long userId, long blogId, VoteTypeDto voteType)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");

        var type = _mapper.Map<VoteType>(voteType);
        blog.AddOrUpdateVote(userId, type);
        blog.RecalculateQualityStatus();
        _blogRepository.Update(blog);
    }

    public void RemoveVote(long userId, long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");

        blog.RemoveVote(userId);
        blog.RecalculateQualityStatus();
        _blogRepository.Update(blog);
    }


    public (int upvotes, int downvotes) GetVotes(long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");
        int up = blog.Votes.Count(v => v.Type == VoteType.Upvote);
        int down = blog.Votes.Count(v => v.Type == VoteType.Downvote);
        return (up, down);
    }

    public BlogVoteDto? GetUserVote(long userId, long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");
        var vote = blog.Votes.FirstOrDefault(v => v.UserId == userId);
        return vote == null ? null : _mapper.Map<BlogVoteDto>(vote);
    }

    public BlogDto RecalculateQualityStatus(long blogId)
    {
        var blog = _blogRepository.GetById(blogId);
        blog.RecalculateQualityStatus();
        var updated = _blogRepository.Update(blog);
        return  _mapper.Map<BlogDto>(updated);
    }

    public List<BlogDto> GetBlogsByQualityStatus(BlogQualityStatusDto statusDto)
    {
        BlogQualityStatus status = statusDto switch
        {
            BlogQualityStatusDto.None => BlogQualityStatus.None,
            BlogQualityStatusDto.Active => BlogQualityStatus.Active,
            BlogQualityStatusDto.Famous => BlogQualityStatus.Famous,
            BlogQualityStatusDto.Closed => BlogQualityStatus.Closed,
            _ => BlogQualityStatus.None
        };

        var blogs = _blogRepository.GetAll()
            .Where(b => b.QualityStatus == status)
            .ToList();

        return blogs.Select(MapBlogWithUsername).ToList();
    }

    private BlogDto MapBlogWithUsername(BlogPost blog)
    {
        var dto = _mapper.Map<BlogDto>(blog);
        dto.Username = _stakeholderService.GetUsername(blog.UserId);
        return dto;
    }
}
