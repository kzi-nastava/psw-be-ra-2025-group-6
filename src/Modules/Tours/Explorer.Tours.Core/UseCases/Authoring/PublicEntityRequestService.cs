using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Core.Integration;
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
    private readonly INotificationPublisher _notificationPublisher;

    public PublicEntityRequestService(
        IPublicEntityRequestRepository requestRepository,
        ITourRepository tourRepository,
        IFacilityRepository facilityRepository,
        INotificationPublisher notificationPublisher,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _tourRepository = tourRepository;
        _facilityRepository = facilityRepository;
        _notificationPublisher = notificationPublisher;
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
        var existingRequests = _requestRepository.GetAll();
        var hasPendingOrApprovedRequest = existingRequests.Any(r => 
            r.EntityType == _mapper.Map<PublicEntityType>(dto.EntityType) && 
            r.EntityId == dto.EntityId &&
            (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Approved));

        if (hasPendingOrApprovedRequest)
            throw new InvalidOperationException("Public request already exists for this entity.");

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

        if (dto.EntityType == PublicEntityTypeDto.Facility)
        {
            var facility = _facilityRepository.Get(dto.EntityId);
            facility.MarkAsPublicRequested(createdRequest.Id);
            _facilityRepository.Update(facility);
        }
        else if (dto.EntityType == PublicEntityTypeDto.KeyPoint)
        {
            var tours = _tourRepository.GetAll();
            var tour = tours.FirstOrDefault(t => t.KeyPoints != null && 
                                                  t.KeyPoints.Any(kp => kp.Id == dto.EntityId));
            
            if (tour == null)
                throw new KeyNotFoundException($"Tour containing KeyPoint {dto.EntityId} not found.");

            var keyPoint = tour.KeyPoints.First(kp => kp.Id == dto.EntityId);
            keyPoint.MarkAsPublicRequested(createdRequest.Id);
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
        request.Approve(adminId);
        _requestRepository.Update(request);

        if (request.EntityType == PublicEntityType.Facility)
        {
            var facility = _facilityRepository.Get(request.EntityId);
            facility.ApprovePublic();
            _facilityRepository.Update(facility);

            // Publish notification via integration abstraction
            _notificationPublisher.PublishNotification(request.AuthorId, adminId, $"Your facility '{facility.Name}' public request has been approved!", facility.Id);
        }
        else if (request.EntityType == PublicEntityType.KeyPoint)
        {
            var tours = _tourRepository.GetAll();
            var tour = tours.FirstOrDefault(t => t.KeyPoints != null && 
                                                  t.KeyPoints.Any(kp => kp.Id == request.EntityId));
            
            if (tour == null)
                throw new KeyNotFoundException($"Tour containing KeyPoint {request.EntityId} not found.");

            var keyPoint = tour.KeyPoints.First(kp => kp.Id == request.EntityId);
            keyPoint.ApprovePublic();
            _tourRepository.Update(tour);

            // publish notification referencing the tour id so frontend can open the tour directly
            _notificationPublisher.PublishNotification(request.AuthorId, adminId, $"Your key point '{keyPoint.Name}' has been approved and is now public.", tour.Id);
        }

        return _mapper.Map<PublicEntityRequestDto>(request);
    }

    public PublicEntityRequestDto RejectRequest(long requestId, long adminId, string comment)
    {
        var request = _requestRepository.Get(requestId);
        request.Reject(adminId, comment);
        _requestRepository.Update(request);

        if (request.EntityType == PublicEntityType.Facility)
        {
            var facility = _facilityRepository.Get(request.EntityId);
            _notificationPublisher.PublishNotification(request.AuthorId, adminId, $"Your facility '{facility.Name}' public request has been rejected: {comment}", facility.Id);
        }
        else if (request.EntityType == PublicEntityType.KeyPoint)
        {
            var tours = _tourRepository.GetAll();
            var tour = tours.FirstOrDefault(t => t.KeyPoints != null && 
                                                  t.KeyPoints.Any(kp => kp.Id == request.EntityId));
            if (tour != null)
            {
                var keyPoint = tour.KeyPoints.First(kp => kp.Id == request.EntityId);
                _notificationPublisher.PublishNotification(request.AuthorId, adminId, $"Your key point '{keyPoint.Name}' public request has been rejected: {comment}", tour.Id);
            }
        }

        return _mapper.Map<PublicEntityRequestDto>(request);
    }
}
