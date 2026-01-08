using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ITourDataProvider _tourDataProvider;
    private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly IMapper _mapper;

    public ShoppingCartService(
        IShoppingCartRepository cartRepository,
        ITourDataProvider tourDataProvider,
        ITourPurchaseTokenRepository tokenRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _tourDataProvider = tourDataProvider;
        _tokenRepository = tokenRepository;
        _mapper = mapper;
    }

    public ShoppingCartDto GetByTouristId(long touristId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
        {
            cart = new ShoppingCart(touristId);
            cart = _cartRepository.Create(cart);
        }
        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public ShoppingCartDto AddItem(long touristId, long tourId, string tourName, double price)
    {
        var tourData = _tourDataProvider.GetTourData(tourId);
        
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
        {
            cart = new ShoppingCart(touristId);
            cart = _cartRepository.Create(cart);
        }

        cart.AddItem(tourData.Id, tourData.Name, tourData.Price, tourData.Status);
        cart = _cartRepository.Update(cart);
        
        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public ShoppingCartDto RemoveItem(long touristId, long tourId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
            throw new KeyNotFoundException($"Shopping cart not found for tourist {touristId}");

        cart.RemoveItem(tourId);
        cart = _cartRepository.Update(cart);
        
        return _mapper.Map<ShoppingCartDto>(cart);
    }

    public List<TourPurchaseTokenDto> Checkout(long touristId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
            throw new KeyNotFoundException($"Shopping cart not found for tourist {touristId}");

        var tokens = cart.Checkout();
        _cartRepository.Update(cart);
        
        var createdTokens = _tokenRepository.CreateBulk(tokens);
        
        return _mapper.Map<List<TourPurchaseTokenDto>>(createdTokens);
    }
}
