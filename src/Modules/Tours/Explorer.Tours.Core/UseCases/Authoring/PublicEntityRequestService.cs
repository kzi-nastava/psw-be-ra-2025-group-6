using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class PublicEntityRequestService : IPublicEntityRequestService
{
    private readonly IPublicEntityRequestRepository _requestRepository;
    private readonly IMapper _mapper;
    private readonly ITourRepository _tourRepository;
    private readonly IFacilityRepository _facilityRepository;

    public PublicEntityRequestService(
        IPublicEntityRequestRepository requestRepository,
        ITourRepository tourRepository,
        IFacilityRepository facilityRepository,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _tourRepository = tourRepository;
        _facilityRepository = facilityRepository;
        _mapper = mapper;
    }

    public PagedResult<PublicEntityRequestDto> GetPaged(int page, int pageSize)
    {
        var result = _requestRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<PublicEntityRequestDto>).ToList();
        return new PagedResult<PublicEntityRequestDto>(items, result.TotalCount);
    }

    public PublicEntityRequestDto Get(long id)
    {
        var result = _requestRepository.Get(id);
        return _mapper.Map<PublicEntityRequestDto>(result);
    }

    public PublicEntityRequestDto CreateRequest(CreatePublicEntityRequestDto dto, long authorId)
    {
        // Check if request already exists for this entity
        var existingRequests = _requestRepository.GetAll();
        var hasPendingOrApprovedRequest = existingRequests.Any(r => 
            r.EntityType == _mapper.Map<PublicEntityType>(dto.EntityType) && 
            r.EntityId == dto.EntityId &&
            (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Approved));

        if (hasPendingOrApprovedRequest)
            throw new InvalidOperationException("Public request already exists for this entity.");

        // Validate entity exists
        if (dto.EntityType == PublicEntityTypeDto.KeyPoint)
        {
            var tours = _tourRepository.GetAll();
            var keyPointExists = tours.Any(t => t.KeyPoints != null && 
                                                 t.KeyPoints.Any(kp => kp.Id == dto.EntityId));
            
            if (!keyPointExists)
                throw new KeyNotFoundException($"KeyPoint with id {dto.EntityId} not found.");
        }
        else if (dto.EntityType == PublicEntityTypeDto.Facility)
        {
            var facility = _facilityRepository.Get(dto.EntityId);
            if (facility == null)
                throw new KeyNotFoundException($"Facility with id {dto.EntityId} not found.");
        }

        var request = new PublicEntityRequest(
            authorId,
            _mapper.Map<PublicEntityType>(dto.EntityType),
            dto.EntityId
        );

        var createdRequest = _requestRepository.Create(request);

        // Mark entity as having a public request
        if (dto.EntityType == PublicEntityTypeDto.Facility)
        {
            var facility = _facilityRepository.Get(dto.EntityId);
            facility.MarkAsPublicRequested(createdRequest.Id);
            _facilityRepository.Update(facility);
        }
        else if (dto.EntityType == PublicEntityTypeDto.KeyPoint)
        {
            // Find the tour containing this KeyPoint
            var tours = _tourRepository.GetAll();
            var tour = tours.FirstOrDefault(t => t.KeyPoints != null && 
                                                  t.KeyPoints.Any(kp => kp.Id == dto.EntityId));
            
            if (tour == null)
                throw new KeyNotFoundException($"Tour containing KeyPoint {dto.EntityId} not found.");

            // Find and mark the KeyPoint
            var keyPoint = tour.KeyPoints.First(kp => kp.Id == dto.EntityId);
            keyPoint.MarkAsPublicRequested(createdRequest.Id);
            
            // Save the tour (EF Core will serialize JSONB automatically)
            _tourRepository.Update(tour);
        }

        return _mapper.Map<PublicEntityRequestDto>(createdRequest);
    }

    public List<PublicEntityRequestDto> GetByAuthor(long authorId)
    {
        var requests = _requestRepository.GetByAuthor(authorId);
        return requests.Select(_mapper.Map<PublicEntityRequestDto>).ToList();
    }

    public List<PublicEntityRequestDto> GetPending()
    {
        var requests = _requestRepository.GetPending();
        return requests.Select(_mapper.Map<PublicEntityRequestDto>).ToList();
    }

    // ADMIN METHODS
    public PublicEntityRequestDto ApproveRequest(long requestId, long adminId)
    {
        var request = _requestRepository.Get(requestId);
        
        // Approve the request
        request.Approve(adminId);
        _requestRepository.Update(request);

        // Mark entity as public based on type
        if (request.EntityType == PublicEntityType.Facility)
        {
            var facility = _facilityRepository.Get(request.EntityId);
            facility.ApprovePublic();
            _facilityRepository.Update(facility);
        }
        else if (request.EntityType == PublicEntityType.KeyPoint)
        {
            // Find the tour containing this KeyPoint
            var tours = _tourRepository.GetAll();
            var tour = tours.FirstOrDefault(t => t.KeyPoints != null && 
                                                  t.KeyPoints.Any(kp => kp.Id == request.EntityId));
            
            if (tour == null)
                throw new KeyNotFoundException($"Tour containing KeyPoint {request.EntityId} not found.");

            // Find and update the KeyPoint
            var keyPoint = tour.KeyPoints.First(kp => kp.Id == request.EntityId);
            keyPoint.ApprovePublic();
            
            // Save the tour (EF Core will serialize JSONB automatically)
            _tourRepository.Update(tour);
        }

        // TODO: Send notification to author
        // _notificationService.Send(new NotificationDto {
        //     UserId = request.AuthorId,
        //     Message = $"Your {request.EntityType} request has been approved!",
        //     Type = "PublicRequestApproved"
        // });

        return _mapper.Map<PublicEntityRequestDto>(request);
    }

    public PublicEntityRequestDto RejectRequest(long requestId, long adminId, string comment)
    {
        var request = _requestRepository.Get(requestId);
        
        // Reject the request
        request.Reject(adminId, comment);
        _requestRepository.Update(request);

        // TODO: Send notification to author with rejection reason
        // _notificationService.Send(new NotificationDto {
        //     UserId = request.AuthorId,
        //     Message = $"Your {request.EntityType} request has been rejected: {comment}",
        //     Type = "PublicRequestRejected"
        // });

        return _mapper.Map<PublicEntityRequestDto>(request);
    }
}
