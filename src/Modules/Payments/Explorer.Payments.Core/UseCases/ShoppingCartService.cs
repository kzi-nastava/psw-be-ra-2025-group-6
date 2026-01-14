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
    private readonly IBundleRepository _bundleRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IPaymentRecordRepository _paymentRecordRepository;
    private readonly IMapper _mapper;

    public ShoppingCartService(
        IShoppingCartRepository cartRepository,
        ITourDataProvider tourDataProvider,
        ITourPurchaseTokenRepository tokenRepository,
        IBundleRepository bundleRepository,
        ICouponRepository couponRepository,
        IPaymentRecordRepository paymentRecordRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _tourDataProvider = tourDataProvider;
        _tokenRepository = tokenRepository;
        _bundleRepository = bundleRepository;
        _couponRepository = couponRepository;
        _paymentRecordRepository = paymentRecordRepository;
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

        // Create payment records for each tour
        foreach (var token in tokens)
        {
            var paymentRecord = new PaymentRecord(
                touristId,
                token.TourId,
                null,
                token.Price,
                token.Price
            );
            _paymentRecordRepository.Create(paymentRecord);
        }
        
        var createdTokens = _tokenRepository.CreateBulk(tokens);
        
        return _mapper.Map<List<TourPurchaseTokenDto>>(createdTokens);
    }

    public List<TourPurchaseTokenDto> CheckoutWithCoupon(long touristId, string couponCode)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
            throw new KeyNotFoundException($"Shopping cart not found for tourist {touristId}");

        if (cart.Items.Count == 0)
            throw new InvalidOperationException("Cannot checkout an empty cart");

        var coupon = _couponRepository.GetByCode(couponCode);
        if (coupon == null || !coupon.IsValid())
            throw new InvalidOperationException("Invalid or expired coupon");

        var items = cart.Items.ToList();
        var tokens = new List<TourPurchaseToken>();

        // Apply coupon logic
        if (coupon.TourId.HasValue)
        {
            // Coupon applies to specific tour
            foreach (var item in items)
            {
                double finalPrice = item.Price;
                int? discountPercent = null;
                string? appliedCouponCode = null;

                if (item.TourId == coupon.TourId.Value)
                {
                    finalPrice = item.Price * (1 - coupon.DiscountPercent / 100.0);
                    discountPercent = coupon.DiscountPercent;
                    appliedCouponCode = couponCode;
                }

                var token = new TourPurchaseToken(touristId, item.TourId, item.TourName, finalPrice);
                tokens.Add(token);

                var paymentRecord = new PaymentRecord(
                    touristId,
                    item.TourId,
                    null,
                    item.Price,
                    finalPrice,
                    discountPercent,
                    appliedCouponCode
                );
                _paymentRecordRepository.Create(paymentRecord);
            }
        }
        else
        {
            // Coupon applies to most expensive tour from the author
            var mostExpensiveItem = items.OrderByDescending(i => i.Price).First();
            
            foreach (var item in items)
            {
                double finalPrice = item.Price;
                int? discountPercent = null;
                string? appliedCouponCode = null;

                if (item.TourId == mostExpensiveItem.TourId)
                {
                    finalPrice = item.Price * (1 - coupon.DiscountPercent / 100.0);
                    discountPercent = coupon.DiscountPercent;
                    appliedCouponCode = couponCode;
                }

                var token = new TourPurchaseToken(touristId, item.TourId, item.TourName, finalPrice);
                tokens.Add(token);

                var paymentRecord = new PaymentRecord(
                    touristId,
                    item.TourId,
                    null,
                    item.Price,
                    finalPrice,
                    discountPercent,
                    appliedCouponCode
                );
                _paymentRecordRepository.Create(paymentRecord);
            }
        }

        // Clear cart
        foreach (var item in items)
        {
            cart.RemoveItem(item.TourId);
        }
        _cartRepository.Update(cart);

        var createdTokens = _tokenRepository.CreateBulk(tokens);
        return _mapper.Map<List<TourPurchaseTokenDto>>(createdTokens);
    }

    public PaymentRecordDto PurchaseBundle(long touristId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);
        
        if (bundle.Status != BundleStatus.PUBLISHED)
            throw new InvalidOperationException("Only published bundles can be purchased");

        // Create payment record for bundle
        var paymentRecord = new PaymentRecord(
            touristId,
            null,
            bundleId,
            bundle.Price,
            bundle.Price
        );
        var createdRecord = _paymentRecordRepository.Create(paymentRecord);

        // Create TourPurchaseToken for each tour in bundle
        var tokens = new List<TourPurchaseToken>();
        foreach (var tourId in bundle.TourIds)
        {
            var tourData = _tourDataProvider.GetTourData(tourId);
            var token = new TourPurchaseToken(touristId, tourId, tourData.Name, 0); // Price is 0 because it's included in bundle
            tokens.Add(token);
        }
        _tokenRepository.CreateBulk(tokens);

        return _mapper.Map<PaymentRecordDto>(createdRecord);
    }
}
