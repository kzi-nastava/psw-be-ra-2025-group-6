using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CheckoutTests : BasePaymentsIntegrationTest
    {
        private const long TOURIST_ID = -21; // Using a tourist from Stakeholders test data

        public CheckoutTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Checkout_creates_tokens_and_empties_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var paymentsDbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            ClearCart(scope);
            controller.AddItem(-3); 
            controller.AddItem(-5); 

            // Act
            var response = controller.Checkout();
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.All(t => t.TouristId == TOURIST_ID).ShouldBeTrue();

            // Assert - Cart is empty
            paymentsDbContext.ChangeTracker.Clear();
            var updatedCart = paymentsDbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == TOURIST_ID);
            updatedCart.ShouldNotBeNull();
            updatedCart.Items.ShouldBeEmpty();

            // Assert - Tokens created in DB
            var tokens = paymentsDbContext.TourPurchaseTokens
                .Where(t => t.TouristId == TOURIST_ID)
                .ToList();
            tokens.Count.ShouldBe(2);
        }

        [Fact]
        public void Checkout_empty_cart_returns_bad_request()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            ClearCart(scope);

            // Act
            var response = controller.Checkout();
            var badRequestResult = response.Result as BadRequestObjectResult;

            // Assert
            badRequestResult.ShouldNotBeNull();
            badRequestResult.StatusCode.ShouldBe(400);
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
    }
}
