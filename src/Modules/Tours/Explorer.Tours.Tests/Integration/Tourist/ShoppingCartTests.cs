using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
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
            var result = ((OkObjectResult)controller.AddItem(-3).Result)?.Value as ShoppingCartDto;

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
            var result = ((OkObjectResult)controller.RemoveItem(-3).Result)?.Value as ShoppingCartDto;

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(0);
        }

        private static ShoppingCartController CreateController(IServiceScope scope)
        {
            return new ShoppingCartController(scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
