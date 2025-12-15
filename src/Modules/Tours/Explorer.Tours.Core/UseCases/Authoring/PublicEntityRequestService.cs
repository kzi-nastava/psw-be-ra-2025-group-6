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
    private readonly IFacilityRepository _facilityRepository;

    public PublicEntityRequestService(
        IPublicEntityRequestRepository requestRepository,
        IFacilityRepository facilityRepository,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
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
        // Check if entity already has a pending or approved request
        switch (dto.EntityType)
        {
            case PublicEntityTypeDto.Facility:
                var facility = _facilityRepository.Get(dto.EntityId);
                if (facility.PublicRequestId.HasValue)
                    throw new InvalidOperationException("Public request already exists for this facility.");
                break;
            // KeyPoint will be handled when we add the repository for it
        }

        var request = new PublicEntityRequest(
            authorId,
            _mapper.Map<PublicEntityType>(dto.EntityType),
            dto.EntityId
        );

        var createdRequest = _requestRepository.Create(request);

        // Mark entity as having a public request
        switch (dto.EntityType)
        {
            case PublicEntityTypeDto.Facility:
                var facility = _facilityRepository.Get(dto.EntityId);
                facility.MarkAsPublicRequested(createdRequest.Id);
                _facilityRepository.Update(facility);
                break;
            // KeyPoint will be handled when we add the repository for it
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
}
