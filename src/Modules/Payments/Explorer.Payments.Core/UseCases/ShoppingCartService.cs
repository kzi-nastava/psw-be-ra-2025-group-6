using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;

namespace Explorer.Payments.Core.UseCases;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ITourDataProvider _tourDataProvider;
    private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly IWalletService _walletService;
    private readonly IPaymentRecordService _paymentRecordService;
    private readonly INotificationDataProvider _notificationDataProvider;
    private readonly IMapper _mapper;

    public ShoppingCartService(
        IShoppingCartRepository cartRepository,
        ITourDataProvider tourDataProvider,
        ITourPurchaseTokenRepository tokenRepository,
        IWalletService walletService,
        IPaymentRecordService paymentRecordService,
        INotificationDataProvider notificationDataProvider,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _tourDataProvider = tourDataProvider;
        _tokenRepository = tokenRepository;
        _walletService = walletService;
        _paymentRecordService = paymentRecordService;
        _notificationDataProvider = notificationDataProvider;
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
        if (cart == null || cart.Items.Count == 0)
            throw new KeyNotFoundException($"Shopping cart is empty or not found for tourist {touristId}");

        var totalPrice = cart.TotalPrice;
        var wallet = _walletService.GetByTouristId(touristId);

        if (wallet.BalanceAc < totalPrice)
        {
            throw new ArgumentException("Insufficient Adventure Coins.");
        }

        _walletService.Pay(touristId, totalPrice);

        foreach (var item in cart.Items)
        {
            var paymentRecord = new PaymentRecordDto
            {
                TouristId = touristId,
                TourId = item.TourId,
                Price = (float)item.Price,
                PurchasedAt = DateTime.UtcNow
            };
            _paymentRecordService.Create(paymentRecord);
        }
        
        _notificationDataProvider.CreateNotification(touristId, 1, "Nova tura je dodata u vašu kolekciju", cart.Id);

        var tokens = cart.Checkout();
        _cartRepository.Update(cart);
        
        var createdTokens = _tokenRepository.CreateBulk(tokens);
        
        return _mapper.Map<List<TourPurchaseTokenDto>>(createdTokens);
    }
}
