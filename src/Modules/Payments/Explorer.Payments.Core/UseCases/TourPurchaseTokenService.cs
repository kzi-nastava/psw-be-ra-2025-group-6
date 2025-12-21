using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class TourPurchaseTokenService : ITourPurchaseTokenService
{
    private readonly ITourPurchaseTokenRepository _repository;
    private readonly IMapper _mapper;

    public TourPurchaseTokenService(ITourPurchaseTokenRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public List<TourPurchaseTokenDto> GetByTouristId(long touristId)
    {
        var tokens = _repository.GetByTouristId(touristId);
        return _mapper.Map<List<TourPurchaseTokenDto>>(tokens);
    }

    public TourPurchaseTokenDto Create(TourPurchaseTokenDto tokenDto)
    {
        var token = _mapper.Map<TourPurchaseToken>(tokenDto);
        var created = _repository.Create(token);
        return _mapper.Map<TourPurchaseTokenDto>(created);
    }

    public List<TourPurchaseTokenDto> CreateBulk(List<TourPurchaseTokenDto> tokenDtos)
    {
        var tokens = _mapper.Map<List<TourPurchaseToken>>(tokenDtos);
        var created = _repository.CreateBulk(tokens);
        return _mapper.Map<List<TourPurchaseTokenDto>>(created);
    }

    public TourPurchaseTokenDto? GetUnusedByTouristAndTour(long touristId, long tourId)
    {
        var token = _repository.GetUnusedByTouristAndTour(touristId, tourId);
        return token == null ? null : _mapper.Map<TourPurchaseTokenDto>(token);
    }

    public TourPurchaseTokenDto MarkAsUsed(long tokenId)
    {
        var token = _repository.GetByTouristAndTour(0, tokenId); // Ovo nije idealno, ali za sada
        token.MarkAsUsed();
        var updated = _repository.Update(token);
        return _mapper.Map<TourPurchaseTokenDto>(updated);
    }
}
