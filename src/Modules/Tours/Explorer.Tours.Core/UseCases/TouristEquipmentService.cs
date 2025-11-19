using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases;

public class TouristEquipmentService : ITouristEquipmentService
{
    private readonly ITouristEquipmentRepository _repository;
    private readonly IMapper _mapper;

    public TouristEquipmentService(ITouristEquipmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public PagedResult<TouristEquipmentDto> GetOwned(long personId, int page, int pageSize)
    {
        var result = _repository.GetOwned(personId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<TouristEquipmentDto>).ToList();
        return new PagedResult<TouristEquipmentDto>(items, result.TotalCount);
    }

    public TouristEquipmentDto Add(TouristEquipmentDto dto)
    {
        var entity = _mapper.Map<TouristEquipment>(dto);
        var created = _repository.Create(entity);
        return _mapper.Map<TouristEquipmentDto>(created);
    }

    public void Remove(long personId, long equipmentId)
    {
        _repository.Delete(personId, equipmentId);
    }
}
