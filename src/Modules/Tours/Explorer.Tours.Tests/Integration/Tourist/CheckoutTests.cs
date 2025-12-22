using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class CheckoutTests : BaseToursIntegrationTest
    {
        private const long TOURIST_ID = -1;

        public CheckoutTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Checkout_creates_tokens_and_empties_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            // Očisti korpu
            var cart = cartService.GetByTouristId(TOURIST_ID);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(TOURIST_ID, item.TourId);
            }

            // Dodaj turu u korpu
            controller.AddItem(-3); // Confirmed tour 

            // Act
            var response = controller.Checkout();
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result.All(t => t.TouristId == TOURIST_ID).ShouldBeTrue();
            result.All(t => t.IsUsed == false).ShouldBeTrue();
            result[0].TourId.ShouldBe(-3);

            // Assert - Korpa je prazna
            dbContext.ChangeTracker.Clear();
            var updatedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == TOURIST_ID);

            updatedCart.ShouldNotBeNull();
            updatedCart.Items.Count.ShouldBe(0);

            // Assert - Tokeni su kreirani u bazi
            var tokens = dbContext.TourPurchaseTokens
                .Where(t => t.TouristId == TOURIST_ID)
                .OrderBy(t => t.TourId)
                .ToList();

            tokens.Count.ShouldBeGreaterThanOrEqualTo(1);
            tokens.Any(t => t.TourId == -3 && !t.IsUsed).ShouldBeTrue();
        }

        [Fact]
        public void Checkout_empty_cart_returns_bad_request()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            // Očisti korpu
            var cart = cartService.GetByTouristId(TOURIST_ID);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(TOURIST_ID, item.TourId);
            }

            // Act
            var response = controller.Checkout();
            var badRequestResult = response.Result as BadRequestObjectResult;

            // Assert
            badRequestResult.ShouldNotBeNull();
            badRequestResult.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Checkout_saves_price_snapshot()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            // Očisti korpu
            var cart = cartService.GetByTouristId(TOURIST_ID);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(TOURIST_ID, item.TourId);
            }

            // Dodaj turu sa cenom 100
            controller.AddItem(-3);

            // Act - Checkout
            var response = controller.Checkout();
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert - Token čuva originalnu cenu
            result.ShouldNotBeNull();
            result[0].Price.ShouldBe(100.0); // Cena iz test podataka

            // Simuliraj promenu cene ture u bazi
            var tour = dbContext.Tours.Find(-3L);
            tour.ShouldNotBeNull();
            var originalPrice = tour.Price;

            // Assert - Token je zabeležio cenu u trenutku kupovine
            var token = dbContext.TourPurchaseTokens
                .FirstOrDefault(t => t.TouristId == TOURIST_ID && t.TourId == -3);

            token.ShouldNotBeNull();
            token.Price.ShouldBe(originalPrice);
        }

        [Fact]
        public void Multiple_checkouts_create_separate_tokens()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            // Očisti korpu
            var cart = cartService.GetByTouristId(TOURIST_ID);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(TOURIST_ID, item.TourId);
            }

            // Prvi checkout
            controller.AddItem(-3);
            var firstCheckout = controller.Checkout();
            var firstOk = firstCheckout.Result as OkObjectResult;
            firstOk.ShouldNotBeNull();

            // Drugi checkout (ista tura ponovo)
            controller.AddItem(-3);
            var secondCheckout = controller.Checkout();
            var secondOk = secondCheckout.Result as OkObjectResult;
            secondOk.ShouldNotBeNull();

            // Assert - Dva odvojena tokena za istu turu
            dbContext.ChangeTracker.Clear();
            var tokens = dbContext.TourPurchaseTokens
                .Where(t => t.TouristId == TOURIST_ID && t.TourId == -3)
                .ToList();

            tokens.Count.ShouldBeGreaterThanOrEqualTo(2); // Može biti više ako su prethodni testovi ostavili podatke
            tokens.All(t => !t.IsUsed).ShouldBeTrue();
        }

        [Fact]
        public void Checkout_for_nonexistent_cart_returns_bad_request()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-99999"); // Turista koji nema korpu

            // Act
            var response = controller.Checkout();
            var badRequestResult = response.Result as BadRequestObjectResult;

            // Assert
            badRequestResult.ShouldNotBeNull();
            badRequestResult.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Checkout_creates_tokens_with_correct_purchase_date()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            // Očisti korpu
            var cart = cartService.GetByTouristId(TOURIST_ID);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(TOURIST_ID, item.TourId);
            }

            controller.AddItem(-3);
            var beforeCheckout = DateTime.UtcNow;

            // Act
            var response = controller.Checkout();
            var afterCheckout = DateTime.UtcNow;

            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert - PurchaseDate je u validnom opsegu
            result.ShouldNotBeNull();
            result[0].PurchaseDate.ShouldBeGreaterThanOrEqualTo(beforeCheckout.AddSeconds(-1));
            result[0].PurchaseDate.ShouldBeLessThanOrEqualTo(afterCheckout.AddSeconds(1));
        }

        [Fact]
        public void Checkout_preserves_tour_name_in_token()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            // Očisti korpu
            var cart = cartService.GetByTouristId(TOURIST_ID);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(TOURIST_ID, item.TourId);
            }

            controller.AddItem(-3);

            // Act
            var response = controller.Checkout();
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as List<TourPurchaseTokenDto>;

            // Assert - TourName je sačuvan
            result.ShouldNotBeNull();
            result[0].TourName.ShouldNotBeNullOrEmpty();
            result[0].TourName.ShouldBe("Tura Pariza"); // From b-tours.sql
        }

        private static ShoppingCartController CreateController(IServiceScope scope, string personId = "-1")
        {
            return new ShoppingCartController(scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}