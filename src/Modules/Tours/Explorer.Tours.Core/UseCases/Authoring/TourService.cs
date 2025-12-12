using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class TourService : ITourService
{
    private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly ITourRepository<Tour> _tourRepository;
    private readonly IMapper _mapper;

    public TourService(ITourRepository<Tour> repository, IMapper mapper, ITourPurchaseTokenRepository tokenRepository)
    {
        _tourRepository = repository;
        _mapper = mapper;
        _tokenRepository = tokenRepository;
    }

    public List<TourDto> GetAll()
    {
        var result = _tourRepository.GetAll();

        var items = _mapper.Map<List<TourDto>>(result);
        return new List<TourDto>(items);
    }

    public PagedResult<TourDto> GetPaged(int page, int pageSize)
    {
        var result = _tourRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }
    public TourDto Get(long id)
    {
        var result = _tourRepository.Get(id);
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Create(TourDto entity)
    {
        entity.Status = TourStatusDto.DRAFT;
        entity.Price = 0;
        var result = _tourRepository.Create(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Update(TourDto entity)
    {
        var result = _tourRepository.Update(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id)
    {
        TourDto item = Get(id);
        if (item.Status != TourStatusDto.DRAFT)
        {
            throw new InvalidOperationException("Only tours in Draft status can be deleted.");
        }
        else
            _tourRepository.Delete(id);
    }

    public List<TourDto> GetAvailableForTourist(long touristId)
    {
        var confirmedTours = _tourRepository.GetAll()
            .Where(t => t.Status == TourStatus.CONFIRMED)
            .ToList();

        var purchasedTourIds = _tokenRepository.GetByTouristId(touristId)
            .Select(t => t.TourId)
            .ToHashSet();

        return _mapper.Map<List<TourDto>>(
            confirmedTours.Where(t => !purchasedTourIds.Contains(t.Id)).ToList()
        );
    }

    public PagedResult<TourDto> GetAvailableForTouristPaged(long touristId, int page, int pageSize)
    {
        
        var confirmedTours = _tourRepository.GetAll()
            .Where(t => t.Status == TourStatus.CONFIRMED)
            .ToList();

        
        var purchasedTourIds = _tokenRepository.GetByTouristId(touristId)
            .Select(t => t.TourId)
            .ToHashSet();

        
        var availableTours = confirmedTours
            .Where(t => !purchasedTourIds.Contains(t.Id))
            .ToList();


        var totalCount = availableTours.Count;
        var pagedTours = availableTours
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        
        var items = _mapper.Map<List<TourDto>>(pagedTours);
        return new PagedResult<TourDto>(items, totalCount);
    }
}
