using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public.Administration;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Blog.Core.UseCases.Administration;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IMapper _mapper;
    private readonly IInternalStakeholderService _stakeholderService;
    private readonly ICommentLikeRepository _likeRepository;
    private readonly ICommentReportRepository _reportRepository;
    private readonly IBlogLocationService _locationService;
    
    public BlogService(IBlogRepository blogRepository, IInternalStakeholderService stakeholderService, IMapper mapper, ICommentLikeRepository likeRepository, ICommentReportRepository reportRepository, IBlogLocationService locationService)
    {
        _blogRepository = blogRepository;
        _stakeholderService = stakeholderService;
        _mapper = mapper;
        _likeRepository = likeRepository;
        _reportRepository = reportRepository;
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
        var authorProfilePicture = _stakeholderService.GetProfilePicture(userId);
        

        blog.AddComment(blogId, userId, authorName, authorProfilePicture, text);
        _blogRepository.Update(blog);

        var comment = blog.Comments.Last();

        return _mapper.Map<CommentDto>(comment);
    }

    public CommentDto EditComment(long blogId, long commentId, long userId, string text)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found.");

        var comment = blog.Comments.First(c => c.Id == commentId);

        blog.EditComment(commentId, userId, text);
        _blogRepository.Update(blog);

        return _mapper.Map<CommentDto>(comment);
    }

    public CommentDto DeleteComment(long blogId, long commentId, long userId) 
    { 
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found");

        var comment = blog.Comments.First(c => c.Id == commentId);

        blog.DeleteComment(commentId, userId);
        _blogRepository.Update(blog);

        return _mapper.Map<CommentDto>(comment);
    }

    public List<CommentDto> GetComments(long blogId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null) throw new Exception("Blog not found");

        var comments = blog.Comments
            .Select((c, index) => new CommentDto
            {    
                Id = c.Id,
                BlogId = c.BlogId,
                UserId = c.UserId,
                AuthorName = c.AuthorName,
                AuthorProfilePicture = c.AuthorProfilePicture,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                LastUpdatedAt = c.LastUpdatedAt,

                LikeCount = _likeRepository.CountLikes(blogId, c.Id),
                IsLikedByMe = _likeRepository.IsLikedByUser(blogId, c.Id, userId),
                IsReportedByMe = _reportRepository.Exists(blogId, c.Id, userId),

                IsHidden = c.IsHidden
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
        dto.AuthorProfilePicture = _stakeholderService.GetProfilePicture(blog.UserId);
        return dto;
    }
    public bool ToggleCommentLike(long blogId, long commentId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");
        if (!blog.Comments.Any(c => c.Id == commentId))
            throw new Exception("Comment not found.");

        return _likeRepository.Toggle(blogId, commentId, userId);
    }

    public int CountCommentLikes(long blogId, long commentId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");
        if (!blog.Comments.Any(c => c.Id == commentId))
            throw new Exception("Comment not found.");

        return _likeRepository.CountLikes(blogId, commentId);
    }

    public bool IsCommentLikedByUser(long blogId, long commentId, long userId)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");
        if (!blog.Comments.Any(c => c.Id == commentId))
            throw new Exception("Comment not found.");

        return _likeRepository.IsLikedByUser(blogId, commentId, userId);
    }

    public void ReportComment(long blogId, long commentId, long userId, ReportTypeDto reason, string? additionalInfo)
    {
        var blog = _blogRepository.GetById(blogId);
        if (blog == null)
            throw new Exception("Blog not found");
        if (!blog.Comments.Any(c => c.Id == commentId))
            throw new Exception("Comment not found.");
        if (_reportRepository.Exists(blogId, commentId, userId))
            throw new InvalidOperationException("You already reported this comment.");

        var domainReason = (ReportType)(int)reason;

        var report = new CommentReport(blogId, commentId, userId, domainReason, additionalInfo);
        _reportRepository.Create(report);
    }

    public bool IsCommentReportedByUser(long blogId, long commentId, long userId)
    {
        return _reportRepository.Exists(blogId, commentId, userId);
    }

    public PagedResult<CommentReportDto> GetByReportStatus(AdminReportStatusDto statusDto, int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var skip = (page - 1) * pageSize;
        var status = (AdminReportStatus)(int)statusDto;

        var reports = _reportRepository.GetByReportStatus(status, skip, pageSize).ToList();
        var total = _reportRepository.CountByStatus(status);

        var blogIds = reports.Select(r => r.BlogId).Distinct().ToList();
        var blogsById = blogIds
            .Select(id => _blogRepository.GetById(id))
            .Where(b => b != null)
            .ToDictionary(b => b.Id, b => b);

        var items = reports.Select(r =>
        {
            blogsById.TryGetValue(r.BlogId, out var blog);
            var comment = blog?.Comments.FirstOrDefault(c => c.Id == r.CommentId);

            return new CommentReportDto
            {
                Id = (int)r.Id,
                BlogId = r.BlogId,
                CommentId = r.CommentId,
                UserId = r.UserId,
                Reason = (ReportTypeDto)(int)r.Reason,
                AdditionalInfo = r.AdditionalInfo,
                CreatedAt = r.CreatedAt,
                ReportStatus = (AdminReportStatusDto)(int)r.ReportStatus,
                ReviewedAt = r.ReviewedAt,
                ReviewerId = r.ReviewerId,
                AdminNote = r.AdminNote,

                CommentAuthorId = comment?.UserId ?? 0,
                CommentAuthorName = comment?.AuthorName ?? "[deleted]",
                CommentText = comment?.Text ?? "",
                CommentCreatedAt = comment?.CreatedAt ?? default
            };
        }).ToList();

        return new PagedResult<CommentReportDto>(items, total);
    }

    public void ApproveCommentReport(long reportId, long adminId, string? note)
    {
        var report = _reportRepository.GetById(reportId);
        if (report == null) throw new NotFoundException("Report not found");

        var blog = _blogRepository.GetById(report.BlogId);
        if (blog == null) throw new NotFoundException("Blog not found");

        report.Approve(adminId, note);
        _reportRepository.Update(report);

        blog.HideComment(report.CommentId, adminId);
        _blogRepository.Update(blog);

        _reportRepository.DeleteOpenByComment(report.BlogId, report.CommentId);
        
    }

    public void DismissCommentReport(long reportId, long adminId, string? note)
    {
        var report = _reportRepository.GetById(reportId);
        if (report == null) throw new NotFoundException("Report not found");

        report.Dismiss(adminId, note);
        _reportRepository.Update(report);
    }

    public PagedResult<BlogDto> GetFollowingBlogs(int page, int pageSize, long userId)
    {
        var followedIds = _stakeholderService.GetFollowedIds(userId);

        if (followedIds == null || !followedIds.Any())
            return new PagedResult<BlogDto>(new List<BlogDto>(), 0);

        var allBlogs = _blogRepository.GetAll();

        var filteredBlogs = allBlogs
            .Where(b => followedIds.Contains(b.UserId) && b.Status == BlogStatus.POSTED)
            .OrderByDescending(b => b.CreatedAt)
            .Select(MapBlogWithUsername)
            .ToList();

        var pagedItems = filteredBlogs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<BlogDto>(pagedItems, filteredBlogs.Count);
    }
}
