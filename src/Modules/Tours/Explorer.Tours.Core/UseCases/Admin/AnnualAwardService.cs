using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Admin;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Admin;

public class AnnualAwardService : IAnnualAwardService
{
    private readonly IAnnualAwardRepository<AnnualAward> _annualAwardRepository;
    private readonly IMapper _mapper;

    public AnnualAwardService(IAnnualAwardRepository<AnnualAward> repository, IMapper mapper)
    {
        _mapper = mapper;
        _annualAwardRepository = repository;
    }
    public AnnualAwardDto Create(AnnualAwardDto dto)
    {
        dto.Status = AwardStatusDto.DRAFT;
        var result = _annualAwardRepository.Create(_mapper.Map<AnnualAward>(dto));
        return _mapper.Map<AnnualAwardDto>(result);
    }

    public void Delete(long id)
    {
        AnnualAwardDto item = Get(id);
        if (item.Status != AwardStatusDto.DRAFT)
        {
            throw new InvalidOperationException("Only awards with draft status can be deleted.");
        }
        else
        {
            _annualAwardRepository.Delete(id);
        }
    }

    public AnnualAwardDto Get(long id)
    {
        var result = _annualAwardRepository.Get(id);
        return _mapper.Map<AnnualAwardDto>(result);
    }

    public PagedResult<AnnualAwardDto> GetPaged(int page, int pageSize)
    {
        var result = _annualAwardRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<AnnualAwardDto>).ToList();

        return new PagedResult<AnnualAwardDto>(items, result.TotalCount);
    }

    public AnnualAwardDto Update(AnnualAwardDto dto)
    {
        var result = _annualAwardRepository.Update(_mapper.Map<AnnualAward>(dto));
        return _mapper.Map<AnnualAwardDto>(result);
    }
}
