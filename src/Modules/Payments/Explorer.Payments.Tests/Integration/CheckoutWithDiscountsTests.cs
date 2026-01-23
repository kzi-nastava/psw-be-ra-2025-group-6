using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class CheckoutWithDiscountsTests : BasePaymentsIntegrationTest
{
    private const long TOURIST_ID = -21;

    public CheckoutWithDiscountsTests(PaymentsTestFactory factory) : base(factory) { }

    [Fact]
    public void Checkout_with_sale_applies_discount_and_charges_correct_amount()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
        var saleService = scope.ServiceProvider.GetRequiredService<ISaleService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        
        ClearCart(scope);
        
        // Create a sale for tour -3 (30% discount)
        var saleDto = new CreateSaleDto
        {
            TourIds = new List<long> { -3 },
            DiscountPercent = 30,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(7)
        };
        saleService.Create(4, saleDto); // AuthorId = 4 for tour -3
        
        // Add tour to cart (original price: 100)
        var tour = tourService.Get(-3);
        controller.AddItem(tour.Id);
        
        // Top up wallet
        walletService.TopUp(TOURIST_ID, 200);
        var walletBefore = walletService.GetByTouristId(TOURIST_ID);
        var balanceBefore = walletBefore.BalanceAc;

        // Act - Checkout with sale
        var response = controller.Checkout();
        var okResult = response.Result as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();
        var tokens = okResult.Value as List<TourPurchaseTokenDto>;
        tokens.ShouldNotBeNull();
        tokens.Count.ShouldBe(1);
        
        // Verify discounted price was charged
        var walletAfter = walletService.GetByTouristId(TOURIST_ID);
        var expectedCharge = 70.0; // 100 - 30% = 70
        var actualCharge = balanceBefore - walletAfter.BalanceAc;
        
        actualCharge.ShouldBe(expectedCharge, 0.01);
    }

    [Fact]
    public void Checkout_with_coupon_charges_wallet()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
        var couponService = scope.ServiceProvider.GetRequiredService<ICouponService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        
        ClearCart(scope);
        
        // Create a coupon for tour -3 (25% discount)
        var couponDto = new CreateCouponDto
        {
            DiscountPercent = 25,
            TourId = -3,
            ValidUntil = DateTime.UtcNow.AddDays(7)
        };
        var createdCoupon = couponService.Create(4, couponDto); // AuthorId = 4 for tour -3
        
        // Add tour to cart (original price: 100)
        var tour = tourService.Get(-3);
        controller.AddItem(tour.Id);
        
        // Top up wallet
        walletService.TopUp(TOURIST_ID, 200);
        var walletBefore = walletService.GetByTouristId(TOURIST_ID);
        var balanceBefore = walletBefore.BalanceAc;

        // Act - Checkout with coupon
        var request = new CheckoutWithCouponDto { CouponCode = createdCoupon.Code };
        var response = controller.CheckoutWithCoupon(request);
        var okResult = response.Result as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();
        var tokens = okResult.Value as List<TourPurchaseTokenDto>;
        tokens.ShouldNotBeNull();
        tokens.Count.ShouldBe(1);
        
        // Verify discounted price was charged from wallet
        var walletAfter = walletService.GetByTouristId(TOURIST_ID);
        var expectedCharge = 75.0; // 100 - 25% = 75
        var actualCharge = balanceBefore - walletAfter.BalanceAc;
        
        actualCharge.ShouldBe(expectedCharge, 0.01);
    }

    [Fact]
    public void Checkout_preview_shows_sale_discount()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var saleService = scope.ServiceProvider.GetRequiredService<ISaleService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        
        ClearCart(scope);
        
        // Create sale
        var saleDto = new CreateSaleDto
        {
            TourIds = new List<long> { -3 },
            DiscountPercent = 20,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(7)
        };
        saleService.Create(4, saleDto);
        
        // Add tour to cart
        var tour = tourService.Get(-3);
        controller.AddItem(tour.Id);

        // Act - Get preview
        var response = controller.GetCheckoutPreview();
        var okResult = response.Result as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();
        var preview = okResult.Value as CheckoutPreviewDto;
        preview.ShouldNotBeNull();
        preview.OriginalTotalPrice.ShouldBe(100.0);
        preview.FinalTotalPrice.ShouldBe(80.0);
        preview.TotalDiscount.ShouldBe(20.0);
        preview.HasDiscount.ShouldBeTrue();
    }

    [Fact]
    public void Checkout_with_coupon_applies_on_top_of_sale()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
        var saleService = scope.ServiceProvider.GetRequiredService<ISaleService>();
        var couponService = scope.ServiceProvider.GetRequiredService<ICouponService>();
        var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
        
        ClearCart(scope);
        
        // Create sale (20% off) for tour -3
        var saleDto = new CreateSaleDto
        {
            TourIds = new List<long> { -3 },
            DiscountPercent = 20,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(7)
        };
        saleService.Create(4, saleDto);
        
        // Create coupon (25% off) for tour -3
        var couponDto = new CreateCouponDto
        {
            DiscountPercent = 25,
            TourId = -3,
            ValidUntil = DateTime.UtcNow.AddDays(7)
        };
        var createdCoupon = couponService.Create(4, couponDto);
        
        // Add tour to cart (original: 100)
        var tour = tourService.Get(-3);
        controller.AddItem(tour.Id);
        
        // Top up wallet
        walletService.TopUp(TOURIST_ID, 200);
        var walletBefore = walletService.GetByTouristId(TOURIST_ID);
        var balanceBefore = walletBefore.BalanceAc;

        // Act - Checkout with coupon
        var request = new CheckoutWithCouponDto { CouponCode = createdCoupon.Code };
        var response = controller.CheckoutWithCoupon(request);
        var okResult = response.Result as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();
        var tokens = okResult.Value as List<TourPurchaseTokenDto>;
        tokens.ShouldNotBeNull();
        
        // Verify combined discount was applied:
        // Original: 100
        // After Sale (20%): 80
        // After Coupon (25% on 80): 60
        var walletAfter = walletService.GetByTouristId(TOURIST_ID);
        var expectedCharge = 60.0;
        var actualCharge = balanceBefore - walletAfter.BalanceAc;
        
        actualCharge.ShouldBe(expectedCharge, 0.01);
    }

    private static ShoppingCartController CreateController(IServiceScope scope)
    {
        return new ShoppingCartController(
            scope.ServiceProvider.GetRequiredService<IShoppingCartService>(),
            scope.ServiceProvider.GetRequiredService<ITourService>()
        )
        {
            ControllerContext = BuildContext(TOURIST_ID.ToString())
        };
    }

    private void ClearCart(IServiceScope scope)
    {
        var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var cart = cartService.GetByTouristId(TOURIST_ID);
        if (cart == null) return;
        foreach (var item in cart.Items.ToList())
        {
            cartService.RemoveItem(TOURIST_ID, item.TourId);
        }
    }
}
