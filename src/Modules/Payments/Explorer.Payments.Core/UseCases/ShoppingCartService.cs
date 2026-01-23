using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Core.UseCases;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ITourDataProvider _tourDataProvider;
    private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly IWalletService _walletService;
    private readonly IPaymentRecordService _paymentRecordService;
    private readonly INotificationDataProvider _notificationDataProvider;
    private readonly IBundleRepository _bundleRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IPaymentRecordRepository _paymentRecordRepository;
    private readonly IMapper _mapper;

    public ShoppingCartService(
        IShoppingCartRepository cartRepository,
        ITourDataProvider tourDataProvider,
        ITourPurchaseTokenRepository tokenRepository,
        IWalletService walletService,
        IPaymentRecordService paymentRecordService,
        INotificationDataProvider notificationDataProvider,
        IBundleRepository bundleRepository,
        ICouponRepository couponRepository,
        IPaymentRecordRepository paymentRecordRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _tourDataProvider = tourDataProvider;
        _tokenRepository = tokenRepository;
        _walletService = walletService;
        _paymentRecordService = paymentRecordService;
        _notificationDataProvider = notificationDataProvider;
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
        var dto = _mapper.Map<ShoppingCartDto>(cart);
        ApplySaleInfo(dto);
        return dto;
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

        var dto = _mapper.Map<ShoppingCartDto>(cart);
        ApplySaleInfo(dto);
        return dto;
    }

    public ShoppingCartDto AddBundle(long touristId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);
        
        if (bundle.Status != BundleStatus.PUBLISHED)
            throw new ArgumentException("Bundle must be published to be added to cart.");

        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
        {
            cart = new ShoppingCart(touristId);
            cart = _cartRepository.Create(cart);
        }

        cart.AddBundle(bundle.Id, bundle.Name, bundle.Price, bundle.Status.ToString());
        cart = _cartRepository.Update(cart);

        var dto = _mapper.Map<ShoppingCartDto>(cart);
        return dto;
    }

    public ShoppingCartDto RemoveItem(long touristId, long tourId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
            throw new KeyNotFoundException($"Shopping cart not found for tourist {touristId}");

        cart.RemoveItem(tourId);
        cart = _cartRepository.Update(cart);

        var dto = _mapper.Map<ShoppingCartDto>(cart);
        ApplySaleInfo(dto);
        return dto;
    }

    public ShoppingCartDto RemoveBundle(long touristId, long bundleId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null)
            throw new KeyNotFoundException($"Shopping cart not found for tourist {touristId}");

        cart.RemoveBundle(bundleId);
        cart = _cartRepository.Update(cart);

        var dto = _mapper.Map<ShoppingCartDto>(cart);
        return dto;
    }

    public List<TourPurchaseTokenDto> Checkout(long touristId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null || (cart.Items.Count == 0 && cart.BundleItems.Count == 0))
            throw new KeyNotFoundException($"Shopping cart is empty or not found for tourist {touristId}");

        var items = cart.Items.ToList();
        var bundleItems = cart.BundleItems.ToList();
        var tokens = new List<TourPurchaseToken>();
        double totalPrice = 0;

        // Process tour items
        foreach (var item in items)
        {
            double finalPrice = item.Price;
            int? discountPercent = null;

            // Check if tour is on sale
            var saleInfo = _tourDataProvider.GetActiveSaleForTour(item.TourId);
            if (saleInfo != null)
            {
                finalPrice = item.Price * (1 - saleInfo.DiscountPercent / 100.0);
                discountPercent = saleInfo.DiscountPercent;
            }

            totalPrice += finalPrice;

            var token = new TourPurchaseToken(touristId, item.TourId, item.TourName, finalPrice);
            tokens.Add(token);

            var paymentRecord = new PaymentRecord(
                touristId,
                item.TourId,
                null,
                item.Price,
                finalPrice,
                discountPercent,
                null
            );
            _paymentRecordRepository.Create(paymentRecord);
        }

        // Process bundle items
        foreach (var bundleItem in bundleItems)
        {
            var bundle = _bundleRepository.Get(bundleItem.BundleId);
            totalPrice += bundle.Price;

            // Create payment record for bundle
            var paymentRecord = new PaymentRecord(
                touristId,
                null,
                bundle.Id,
                bundle.Price,
                bundle.Price
            );
            _paymentRecordRepository.Create(paymentRecord);

            // Create tokens for all tours in bundle
            foreach (var tourId in bundle.TourIds)
            {
                var tourData = _tourDataProvider.GetTourData(tourId);
                var token = new TourPurchaseToken(touristId, tourId, tourData.Name, 0);
                tokens.Add(token);
            }
        }

        var wallet = _walletService.GetByTouristId(touristId);
        if (wallet.BalanceAc < totalPrice)
        {
            throw new ArgumentException("Insufficient Adventure Coins.");
        }

        _walletService.Pay(touristId, totalPrice);
        
        _notificationDataProvider.CreateNotification(touristId, 1, "Nova tura je dodata u vašu kolekciju", cart.Id);

        // Clear cart
        foreach (var item in items)
        {
            cart.RemoveItem(item.TourId);
        }
        foreach (var bundleItem in bundleItems)
        {
            cart.RemoveBundle(bundleItem.BundleId);
        }
        _cartRepository.Update(cart);
        
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
        double totalPrice = 0;

        // Apply coupon logic
        if (coupon.TourId.HasValue)
        {
            // Coupon applies to specific tour
            foreach (var item in items)
            {
                double originalPrice = item.Price;
                double priceAfterSale = item.Price;
                double finalPrice = item.Price;
                int? saleDiscountPercent = null;
                int? couponDiscountPercent = null;
                string? appliedCouponCode = null;

                // STEP 1: Apply sale discount first (if exists)
                var saleInfo = _tourDataProvider.GetActiveSaleForTour(item.TourId);
                if (saleInfo != null)
                {
                    priceAfterSale = item.Price * (1 - saleInfo.DiscountPercent / 100.0);
                    saleDiscountPercent = saleInfo.DiscountPercent;
                    finalPrice = priceAfterSale;
                }

                // STEP 2: Apply coupon on top of sale price (if tour matches)
                if (item.TourId == coupon.TourId.Value)
                {
                    finalPrice = priceAfterSale * (1 - coupon.DiscountPercent / 100.0);
                    couponDiscountPercent = coupon.DiscountPercent;
                    appliedCouponCode = couponCode;
                }

                totalPrice += finalPrice;

                var token = new TourPurchaseToken(touristId, item.TourId, item.TourName, finalPrice);
                tokens.Add(token);

                // Calculate total discount percent
                int? totalDiscountPercent = null;
                if (saleDiscountPercent.HasValue && couponDiscountPercent.HasValue)
                {
                    // Combined discount
                    totalDiscountPercent = (int)Math.Round(
                        (1 - finalPrice / originalPrice) * 100
                    );
                }
                else if (saleDiscountPercent.HasValue)
                {
                    totalDiscountPercent = saleDiscountPercent;
                }
                else if (couponDiscountPercent.HasValue)
                {
                    totalDiscountPercent = couponDiscountPercent;
                }

                var paymentRecord = new PaymentRecord(
                    touristId,
                    item.TourId,
                    null,
                    originalPrice,
                    finalPrice,
                    totalDiscountPercent,
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
                double originalPrice = item.Price;
                double priceAfterSale = item.Price;
                double finalPrice = item.Price;
                int? saleDiscountPercent = null;
                int? couponDiscountPercent = null;
                string? appliedCouponCode = null;

                // STEP 1: Apply sale discount first (if exists)
                var saleInfo = _tourDataProvider.GetActiveSaleForTour(item.TourId);
                if (saleInfo != null)
                {
                    priceAfterSale = item.Price * (1 - saleInfo.DiscountPercent / 100.0);
                    saleDiscountPercent = saleInfo.DiscountPercent;
                    finalPrice = priceAfterSale;
                }

                // STEP 2: Apply coupon on top of sale price (if most expensive)
                if (item.TourId == mostExpensiveItem.TourId)
                {
                    finalPrice = priceAfterSale * (1 - coupon.DiscountPercent / 100.0);
                    couponDiscountPercent = coupon.DiscountPercent;
                    appliedCouponCode = couponCode;
                }

                totalPrice += finalPrice;

                var token = new TourPurchaseToken(touristId, item.TourId, item.TourName, finalPrice);
                tokens.Add(token);

                // Calculate total discount percent
                int? totalDiscountPercent = null;
                if (saleDiscountPercent.HasValue && couponDiscountPercent.HasValue)
                {
                    totalDiscountPercent = (int)Math.Round(
                        (1 - finalPrice / originalPrice) * 100
                    );
                }
                else if (saleDiscountPercent.HasValue)
                {
                    totalDiscountPercent = saleDiscountPercent;
                }
                else if (couponDiscountPercent.HasValue)
                {
                    totalDiscountPercent = couponDiscountPercent;
                }

                var paymentRecord = new PaymentRecord(
                    touristId,
                    item.TourId,
                    null,
                    originalPrice,
                    finalPrice,
                    totalDiscountPercent,
                    appliedCouponCode
                );
                _paymentRecordRepository.Create(paymentRecord);
            }
        }

        // Charge wallet with total discounted price
        var wallet = _walletService.GetByTouristId(touristId);
        if (wallet.BalanceAc < totalPrice)
        {
            throw new ArgumentException("Insufficient Adventure Coins.");
        }

        _walletService.Pay(touristId, totalPrice);
        
        _notificationDataProvider.CreateNotification(touristId, 1, "Nova tura je dodata u vašu kolekciju", cart.Id);

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

    private void ApplySaleInfo(ShoppingCartDto cart)
    {
        if (cart?.Items == null || cart.Items.Count == 0) return;

        foreach (var item in cart.Items)
        {
            item.OriginalPrice = item.Price;
            var saleInfo = _tourDataProvider.GetActiveSaleForTour(item.TourId);
            if (saleInfo != null)
            {
                item.IsOnSale = true;
                item.DiscountPercent = saleInfo.DiscountPercent;
                item.DiscountedPrice = item.Price * (1 - saleInfo.DiscountPercent / 100.0);
                item.SaleStartDate = saleInfo.StartDate;
                item.SaleEndDate = saleInfo.EndDate;
            }
            else
            {
                item.IsOnSale = false;
                item.DiscountPercent = null;
                item.DiscountedPrice = null;
                item.SaleStartDate = null;
                item.SaleEndDate = null;
            }
        }
    }

    public CheckoutPreviewDto GetCheckoutPreview(long touristId)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null || cart.Items.Count == 0)
            throw new KeyNotFoundException($"Shopping cart is empty or not found for tourist {touristId}");

        var previewItems = new List<CheckoutItemPreviewDto>();
        double originalTotal = 0;
        double finalTotal = 0;

        foreach (var item in cart.Items)
        {
            double finalPrice = item.Price;
            int? discountPercent = null;
            bool hasDiscount = false;

            // Check if tour is on sale
            var saleInfo = _tourDataProvider.GetActiveSaleForTour(item.TourId);
            if (saleInfo != null)
            {
                finalPrice = item.Price * (1 - saleInfo.DiscountPercent / 100.0);
                discountPercent = saleInfo.DiscountPercent;
                hasDiscount = true;
            }

            originalTotal += item.Price;
            finalTotal += finalPrice;

            previewItems.Add(new CheckoutItemPreviewDto
            {
                TourId = item.TourId,
                TourName = item.TourName,
                OriginalPrice = item.Price,
                FinalPrice = finalPrice,
                DiscountPercent = discountPercent,
                HasDiscount = hasDiscount
            });
        }

        return new CheckoutPreviewDto
        {
            OriginalTotalPrice = originalTotal,
            FinalTotalPrice = finalTotal,
            TotalDiscount = originalTotal - finalTotal,
            DiscountPercent = null,
            CouponCode = null,
            HasDiscount = originalTotal != finalTotal,
            Items = previewItems
        };
    }

    public CheckoutPreviewDto GetCheckoutPreviewWithCoupon(long touristId, string couponCode)
    {
        var cart = _cartRepository.GetByTouristId(touristId);
        if (cart == null || cart.Items.Count == 0)
            throw new KeyNotFoundException($"Shopping cart is empty or not found for tourist {touristId}");

        var coupon = _couponRepository.GetByCode(couponCode);
        if (coupon == null || !coupon.IsValid())
            throw new InvalidOperationException("Invalid or expired coupon");

        var items = cart.Items.ToList();
        var previewItems = new List<CheckoutItemPreviewDto>();
        double originalTotal = 0;
        double finalTotal = 0;

        if (coupon.TourId.HasValue)
        {
            // Coupon applies to specific tour
            foreach (var item in items)
            {
                double originalPrice = item.Price;
                double priceAfterSale = item.Price;
                double finalPrice = item.Price;
                int? totalDiscountPercent = null;
                bool hasDiscount = false;

                // STEP 1: Apply sale discount first (if exists)
                var saleInfo = _tourDataProvider.GetActiveSaleForTour(item.TourId);
                if (saleInfo != null)
                {
                    priceAfterSale = item.Price * (1 - saleInfo.DiscountPercent / 100.0);
                    finalPrice = priceAfterSale;
                    hasDiscount = true;
                }

                // STEP 2: Apply coupon on top of sale price (if tour matches)
                if (item.TourId == coupon.TourId.Value)
                {
                    finalPrice = priceAfterSale * (1 - coupon.DiscountPercent / 100.0);
                    hasDiscount = true;
                }

                // Calculate total discount percent
                if (finalPrice < originalPrice)
                {
                    totalDiscountPercent = (int)Math.Round(
                        (1 - finalPrice / originalPrice) * 100
                    );
                }

                originalTotal += originalPrice;
                finalTotal += finalPrice;

                previewItems.Add(new CheckoutItemPreviewDto
                {
                    TourId = item.TourId,
                    TourName = item.TourName,
                    OriginalPrice = originalPrice,
                    FinalPrice = finalPrice,
                    DiscountPercent = totalDiscountPercent,
                    HasDiscount = hasDiscount
                });
            }
        }
        else
        {
            // Coupon applies to most expensive tour from the author
            var mostExpensiveItem = items.OrderByDescending(i => i.Price).First();

            foreach (var item in items)
            {
                double originalPrice = item.Price;
                double priceAfterSale = item.Price;
                double finalPrice = item.Price;
                int? totalDiscountPercent = null;
                bool hasDiscount = false;

                // STEP 1: Apply sale discount first (if exists)
                var saleInfo = _tourDataProvider.GetActiveSaleForTour(item.TourId);
                if (saleInfo != null)
                {
                    priceAfterSale = item.Price * (1 - saleInfo.DiscountPercent / 100.0);
                    finalPrice = priceAfterSale;
                    hasDiscount = true;
                }

                // STEP 2: Apply coupon on top of sale price (if most expensive)
                if (item.TourId == mostExpensiveItem.TourId)
                {
                    finalPrice = priceAfterSale * (1 - coupon.DiscountPercent / 100.0);
                    hasDiscount = true;
                }

                // Calculate total discount percent
                if (finalPrice < originalPrice)
                {
                    totalDiscountPercent = (int)Math.Round(
                        (1 - finalPrice / originalPrice) * 100
                    );
                }

                originalTotal += originalPrice;
                finalTotal += finalPrice;

                previewItems.Add(new CheckoutItemPreviewDto
                {
                    TourId = item.TourId,
                    TourName = item.TourName,
                    OriginalPrice = originalPrice,
                    FinalPrice = finalPrice,
                    DiscountPercent = totalDiscountPercent,
                    HasDiscount = hasDiscount
                });
            }
        }

        return new CheckoutPreviewDto
        {
            OriginalTotalPrice = originalTotal,
            FinalTotalPrice = finalTotal,
            TotalDiscount = originalTotal - finalTotal,
            DiscountPercent = coupon.DiscountPercent,
            CouponCode = couponCode,
            HasDiscount = true,
            Items = previewItems
        };
    }
}
