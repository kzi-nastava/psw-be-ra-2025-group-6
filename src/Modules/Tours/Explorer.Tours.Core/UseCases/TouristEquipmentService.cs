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
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public TouristEquipmentService(ITouristEquipmentRepository repository, IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _repository = repository;
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    public PagedResult<TouristEquipmentDto> GetOwned(long personId, int page, int pageSize)
    {
        var result = _repository.GetOwned(personId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<TouristEquipmentDto>).ToList();

        var equipmentIds = items.Select(i => i.EquipmentId).Distinct().ToList();
        var equipmentMap = new Dictionary<long, (string Name, string? Description)>();

        foreach (var id in equipmentIds)
        {
            try
            {
                var e = _equipmentRepository.Get(id);
                equipmentMap[id] = (e.Name, e.Description);
            }
            catch
            {
                // ignore missing equipment
            }
        }

        foreach (var dto in items)
        {
            if (equipmentMap.TryGetValue(dto.EquipmentId, out var info))
            {
                dto.Name = info.Name;
                dto.Description = info.Description;
            }
        }

        return new PagedResult<TouristEquipmentDto>(items, result.TotalCount);
    }

    public TouristEquipmentDto Add(TouristEquipmentDto dto)
    {
        var entity = _mapper.Map<TouristEquipment>(dto);
        var created = _repository.Create(entity);

        var outDto = _mapper.Map<TouristEquipmentDto>(created);

        try
        {
            var e = _equipmentRepository.Get(outDto.EquipmentId);
            outDto.Name = e.Name;
            outDto.Description = e.Description;
        }
        catch
        {
            // ignore
        }

        return outDto;
    }

    public void Remove(long personId, long equipmentId)
    {
        _repository.Delete(personId, equipmentId);
    }

    public PagedResult<EquipmentDto> GetAvailableEquipment(int page, int pageSize)
    {
        var assigned = _repository.GetAssignedEquipmentIds();
        var result = _equipmentRepository.GetPagedExcluding(assigned, page, pageSize);
        var items = result.Results.Select(_mapper.Map<EquipmentDto>).ToList();
        return new PagedResult<EquipmentDto>(items, result.TotalCount);
    }
}
