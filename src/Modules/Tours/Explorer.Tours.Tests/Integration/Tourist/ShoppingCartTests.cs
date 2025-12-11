using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit; 
using Explorer.Tours.Core.Domain.RepositoryInterfaces; 

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class ShoppingCartTests : BaseToursIntegrationTest
    {
        public ShoppingCartTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Get_cart_empty()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        
            var cart = cartService.GetByTouristId(-1);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(-1, item.TourId);
            }

            // Act
            var response = controller.Get();
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as ShoppingCartDto;

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
            result.Items.Count.ShouldBe(0);
            result.TotalPrice.ShouldBe(0);
        }

        [Fact]
        public void Adds_item_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
            var cart = cartService.GetByTouristId(-1);
            if (cart != null)
            {
                foreach (var item in cart.Items)
                {
                    cartService.RemoveItem(-1, item.TourId);
                }
            }

            // Act
            var response = controller.AddItem(-3);
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as ShoppingCartDto;

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
        }

        [Fact]
        public void Removes_item_from_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
            var cart = cartService.GetByTouristId(-1);
            if (cart != null)
            {
                foreach (var item in cart.Items)
                {
                    cartService.RemoveItem(-1, item.TourId);
                }
            }
            controller.AddItem(-3);

            // Act
            var response = controller.RemoveItem(-3);
            var okResult = response.Result as OkObjectResult;
            var result = okResult?.Value as ShoppingCartDto;

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
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
            var cart = cartService.GetByTouristId(-1);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(-1, item.TourId);
            }

            
            var firstResponse = controller.AddItem(-3);
            var firstOk = firstResponse.Result as OkObjectResult;
            firstOk.ShouldNotBeNull();

            // Act 
            var secondResponse = controller.AddItem(-3);
            var conflictResult = secondResponse.Result as ConflictObjectResult;

            // Assert
            conflictResult.ShouldNotBeNull();
            conflictResult.StatusCode.ShouldBe(409);
        }

        [Fact]
        public void Cannot_add_draft_tour_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var context = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            var cart = cartService.GetByTouristId(-1);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(-1, item.TourId);
            }

            
            var draftTourId = context.Tours
                .Where(t => t.Status == TourStatus.DRAFT)
                .Select(t => t.Id)
                .First();

            // Act
            var response = controller.AddItem(draftTourId);
            var badRequest = response.Result as BadRequestObjectResult;

            // Assert
            badRequest.ShouldNotBeNull();
            badRequest.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Cannot_add_archived_tour_to_cart()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var context = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var cartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

            var cart = cartService.GetByTouristId(-1);
            foreach (var item in cart.Items)
            {
                cartService.RemoveItem(-1, item.TourId);
            }

            
            var archivedTourId = context.Tours
                .Where(t => t.Status == TourStatus.ARCHIVED)
                .Select(t => t.Id)
                .First();

            // Act
            var response = controller.AddItem(archivedTourId);
            var badRequest = response.Result as BadRequestObjectResult;

            // Assert
            badRequest.ShouldNotBeNull();
            badRequest.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Removing_from_nonexistent_cart_returns_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12345");

            // Act
            var response = controller.RemoveItem(-3);
            var notFoundResult = response.Result as NotFoundObjectResult;

            // Assert
            notFoundResult.ShouldNotBeNull();
            notFoundResult.StatusCode.ShouldBe(404);
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