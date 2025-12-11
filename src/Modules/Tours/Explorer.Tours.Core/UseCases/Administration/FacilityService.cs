using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IMapper _mapper;

    public FacilityService(IFacilityRepository repository, IMapper mapper)
    {
        _facilityRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<FacilityDto> GetPaged(int page, int pageSize)
    {
        var result = _facilityRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<FacilityDto>).ToList();
        return new PagedResult<FacilityDto>(items, result.TotalCount);
    }

    public FacilityDto Create(FacilityDto entity)
    {
        var result = _facilityRepository.Create(_mapper.Map<Facility>(entity));
        return _mapper.Map<FacilityDto>(result);
    }

    public FacilityDto Update(FacilityDto entity)
    {
        var result = _facilityRepository.Update(_mapper.Map<Facility>(entity));
        return _mapper.Map<FacilityDto>(result);
    }

    public void Delete(long Id)
    {
        _facilityRepository.Delete(Id);
    }

}

