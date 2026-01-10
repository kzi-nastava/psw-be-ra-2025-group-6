using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class ShoppingCartTests : BasePaymentsIntegrationTest
    {
        private const long TOURIST_ID = -21;

        public ShoppingCartTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_cart_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = (controller.Get().Result as OkObjectResult)?.Value as ShoppingCartDto;

            // Assert
            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(TOURIST_ID);
        }

        [Fact]
        public void Adds_item_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            ClearCart(scope);
            const long tourId = -3; // A confirmed tour from test data

            // Act
            var result = (controller.AddItem(tourId).Result as OkObjectResult)?.Value as ShoppingCartDto;

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items.First().TourId.ShouldBe(tourId);
        }

        [Fact]
        public void Removes_item_from_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            const long tourId = -3;
            controller.AddItem(tourId); 

            // Act
            var result = (controller.RemoveItem(tourId).Result as OkObjectResult)?.Value as ShoppingCartDto;

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(0);
        }

        [Fact]
        public void Adding_same_tour_twice_returns_conflict()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            ClearCart(scope);
            const long tourId = -3;
            controller.AddItem(tourId);

            // Act 
            var result = controller.AddItem(tourId).Result as ConflictObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(409);
        }

        [Fact]
        public void Cannot_add_draft_tour_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            ClearCart(scope);

            var draftTourId = toursContext.Tours
                .First(t => t.Status == TourStatus.DRAFT).Id;

            // Act
            var result = controller.AddItem(draftTourId).Result as BadRequestObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Cannot_add_archived_tour_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            ClearCart(scope);

            var archivedTourId = toursContext.Tours
                .First(t => t.Status == TourStatus.ARCHIVED).Id;

            // Act
            var result = controller.AddItem(archivedTourId).Result as BadRequestObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Removing_from_nonexistent_cart_returns_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-9999"); // Non-existent tourist

            // Act
            var result = controller.RemoveItem(-3).Result as NotFoundObjectResult;

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        private static ShoppingCartController CreateController(IServiceScope scope, string personId = null)
        {
            var id = personId ?? TOURIST_ID.ToString();
            return new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>()
            )
            {
                ControllerContext = BuildContext(id)
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
}
