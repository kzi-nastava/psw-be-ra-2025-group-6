using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class MonumentService : IMonumentService
{
    private readonly IMonumentRepository _monumentRepository;
    private readonly IMapper _mapper;

    public MonumentService(IMonumentRepository repository, IMapper mapper)
    {
        _monumentRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<MonumentDto> GetPaged(int page, int pageSize)
    {
        var result = _monumentRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<MonumentDto>).ToList();
        return new PagedResult<MonumentDto>(items, result.TotalCount);
    }

    public MonumentDto Get(long id)
    {
        var result = _monumentRepository.Get(id);
        return _mapper.Map<MonumentDto>(result);
    }

    public MonumentDto Create(MonumentDto entity)
    {
        var result = _monumentRepository.Create(_mapper.Map<Monument>(entity));
        return _mapper.Map<MonumentDto>(result);
    }

    public MonumentDto Update(MonumentDto entity)
    {
        var result = _monumentRepository.Update(_mapper.Map<Monument>(entity));
        return _mapper.Map<MonumentDto>(result);
    }

    public void Delete(long id)
    {
        _monumentRepository.Delete(id);
    }
}
