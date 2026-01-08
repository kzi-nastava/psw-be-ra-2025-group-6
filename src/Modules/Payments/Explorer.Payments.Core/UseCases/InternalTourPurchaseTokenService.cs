using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class InternalTourPurchaseTokenService : IInternalTourPurchaseTokenService
{
    private readonly ITourPurchaseTokenRepository _tokenRepository;

    public InternalTourPurchaseTokenService(ITourPurchaseTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public bool DoesTouristHaveUnusedToken(long touristId, long tourId)
    {
        var token = _tokenRepository.GetUnusedByTouristAndTour(touristId, tourId);
        return token != null && !token.IsUsed;
    }

    public void MarkTokenAsUsed(long touristId, long tourId)
    {
        var token = _tokenRepository.GetUnusedByTouristAndTour(touristId, tourId);
        if (token != null)
        {
            token.MarkAsUsed();
            _tokenRepository.Update(token);
        }
    }

    public List<long> GetPurchasedTourIds(long touristId)
    {
        return _tokenRepository.GetByTouristId(touristId).Select(t => t.TourId).ToList();
    }
}
