using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class TourService : ITourService
{
    private readonly ICrudRepository<Tour> _crudRepository;
    private readonly IMapper _mapper;

    public TourService(ICrudRepository<Tour> repository, IMapper mapper)
    {
        _crudRepository = repository;
        _mapper = mapper;
    }

    /*public List<TourDto> GetAll() {
        var result = _crudRepository.GetAll();

        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new List<TourDto>(items);
    }*/

    public PagedResult<TourDto> GetPaged(int page, int pageSize)
    {
        var result = _crudRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }
    public TourDto Get(long id)
    {
        var result = _crudRepository.Get(id);
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Create(TourDto entity)
    {
        entity.Status = TourStatusDto.DRAFT;
        entity.Price = 0;
        var result = _crudRepository.Create(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Update(TourDto entity)
    {
        var result = _crudRepository.Update(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id)
    {
        TourDto item = Get(id);
        if (item.Status != TourStatusDto.DRAFT) {
            throw new InvalidOperationException("Only tours in Draft status can be deleted.");
        }
        else
            _crudRepository.Delete(id);
    }
}
