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
using Microsoft.EntityFrameworkCore;
using System;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CheckoutTests : BasePaymentsIntegrationTest
    {
        private const long TOURIST_ID = -21; 

        public CheckoutTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Successful_checkout_with_payment()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
            ClearCart(scope);
            var tour1 = tourService.Get(-3);
            var tour2 = tourService.Get(-5);
            controller.AddItem(tour1.Id);
            controller.AddItem(tour2.Id);
            walletService.TopUp(TOURIST_ID, 500);

            // Act
            var response = controller.Checkout();
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);

            // Assert - Wallet balance
            var updatedWallet = walletService.GetByTouristId(TOURIST_ID);
            updatedWallet.BalanceAc.ShouldBe(300); 

            // Assert - PaymentRecords created
            var paymentRecords = paymentsDbContext.PaymentRecords.Where(pr => pr.TouristId == TOURIST_ID).ToList();
            paymentRecords.Count.ShouldBe(2);

            // Assert - Notification created
            var notifications = notificationService.GetUnreadByRecipient(TOURIST_ID);
            notifications.ShouldContain(n => n.Content == "Nova tura je dodata u vašu kolekciju");
            
            // Assert - Cart is empty
            paymentsDbContext.ChangeTracker.Clear();
            var updatedCart = paymentsDbContext.ShoppingCarts.Include(c => c.Items).FirstOrDefault(c => c.TouristId == TOURIST_ID);
            updatedCart.ShouldNotBeNull();
            updatedCart.Items.ShouldBeEmpty();
        }

        [Fact]
        public void Checkout_succeeds_with_exact_balance()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
            ClearCart(scope);

            var tour1 = tourService.Get(-3);
            var tour2 = tourService.Get(-5);
            controller.AddItem(tour1.Id);
            controller.AddItem(tour2.Id);

            var total = tour1.Price + tour2.Price;
            EnsureWalletBalance(walletService, TOURIST_ID, total);

            var response = controller.Checkout();
            var okResult = response.Result as OkObjectResult;
            var tokens = okResult?.Value as List<TourPurchaseTokenDto>;

            tokens.ShouldNotBeNull();
            tokens.Count.ShouldBe(2);
            tokens.ShouldContain(t => t.TourId == tour1.Id && t.TourName == tour1.Name);
            tokens.ShouldContain(t => t.TourId == tour2.Id && t.TourName == tour2.Name);

            var updatedWallet = walletService.GetByTouristId(TOURIST_ID);
            updatedWallet.BalanceAc.ShouldBe(0);
        }

        [Fact]
        public void Checkout_fails_due_to_insufficient_funds()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
            ClearCart(scope);
            var tour1 = tourService.Get(-3);
            var tour2 = tourService.Get(-5);
            controller.AddItem(tour1.Id);
            controller.AddItem(tour2.Id);
            walletService.TopUp(TOURIST_ID, 50);

            // Act
            var response = controller.Checkout();

            // Assert
            response.Result.ShouldBeOfType<BadRequestObjectResult>();
            var objectResult = (BadRequestObjectResult)response.Result;
            objectResult.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Checkout_after_success_returns_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
            ClearCart(scope);

            var tour1 = tourService.Get(-3);
            controller.AddItem(tour1.Id);
            EnsureWalletBalance(walletService, TOURIST_ID, tour1.Price);

            controller.Checkout();

            var response = controller.Checkout();
            var notFoundResult = response.Result as NotFoundObjectResult;

            notFoundResult.ShouldNotBeNull();
            notFoundResult.StatusCode.ShouldBe(404);
        }

        [Fact]
        public void Checkout_empty_cart_returns_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            ClearCart(scope);

            // Act
            var response = controller.Checkout();
            var notFoundResult = response.Result as NotFoundObjectResult;

            // Assert
            notFoundResult.ShouldNotBeNull();
            notFoundResult.StatusCode.ShouldBe(404);
        }
        
        [Fact]
        public void Checkout_for_nonexistent_tourist_returns_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-9999");
            
            // Act
            var response = controller.Checkout();
            var notFoundResult = response.Result as NotFoundObjectResult;

            // Assert
            notFoundResult.ShouldNotBeNull();
            notFoundResult.StatusCode.ShouldBe(404);
        }

        private static ShoppingCartController CreateController(IServiceScope scope, string personId = null)
        {
            var id = personId ?? TOURIST_ID.ToString();
            var controller = new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>()
            );
            controller.ControllerContext = BuildContext(id);
            return controller;
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

        private static void EnsureWalletBalance(IWalletService walletService, long touristId, double targetBalance)
        {
            var current = walletService.GetByTouristId(touristId).BalanceAc;
            if (Math.Abs(current - targetBalance) < 0.01)
            {
                return;
            }

            if (current > targetBalance)
            {
                walletService.Pay(touristId, current - targetBalance);
                return;
            }

            walletService.TopUp(touristId, targetBalance - current);
        }
    }
}
