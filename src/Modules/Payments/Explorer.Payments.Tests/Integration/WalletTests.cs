using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class WalletTests : BasePaymentsIntegrationTest
    {
        private const long TOURIST_ID = -21;
        private const long ADMIN_ID = -1;

        public WalletTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_wallet_with_zero_balance()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateWalletController(scope, TOURIST_ID);

            var actionResult = controller.Get();
            var result = ExtractWallet(actionResult);

            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(TOURIST_ID);
            result.BalanceAc.ShouldBe(0);
        }

        [Fact]
        public void Admin_top_up_creates_wallet()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminWalletController(scope, ADMIN_ID);
            var dto = new WalletTopUpDto { TouristId = TOURIST_ID, Amount = 25 };

            var actionResult = controller.TopUp(dto);
            var result = ExtractWallet(actionResult);

            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(TOURIST_ID);
            result.BalanceAc.ShouldBe(25);
        }

        [Fact]
        public void Admin_top_up_increases_balance()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminWalletController(scope, ADMIN_ID);

            controller.TopUp(new WalletTopUpDto { TouristId = TOURIST_ID, Amount = 10 });
            var actionResult = controller.TopUp(new WalletTopUpDto { TouristId = TOURIST_ID, Amount = 15 });
            var result = ExtractWallet(actionResult);

            result.ShouldNotBeNull();
            result.BalanceAc.ShouldBe(25);
        }

        [Fact]
        public void Admin_top_up_rejects_negative_amount()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminWalletController(scope, ADMIN_ID);

            var actionResult = controller.TopUp(new WalletTopUpDto { TouristId = TOURIST_ID, Amount = -5 });
            var result = actionResult.Result as BadRequestObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void Admin_top_up_rejects_zero_amount()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminWalletController(scope, ADMIN_ID);

            var actionResult = controller.TopUp(new WalletTopUpDto { TouristId = TOURIST_ID, Amount = 0 });
            var result = actionResult.Result as BadRequestObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        private static WalletController CreateWalletController(IServiceScope scope, long personId)
        {
            return new WalletController(
                scope.ServiceProvider.GetRequiredService<IWalletService>()
            )
            {
                ControllerContext = BuildContext(personId.ToString())
            };
        }

        private static AdminWalletController CreateAdminWalletController(IServiceScope scope, long personId)
        {
            return new AdminWalletController(
                scope.ServiceProvider.GetRequiredService<IWalletService>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>()
            )
            {
                ControllerContext = BuildContext(personId.ToString())
            };
        }

        private static WalletDto? ExtractWallet(ActionResult<WalletDto> actionResult)
        {
            if (actionResult.Result is OkObjectResult ok)
            {
                return ok.Value as WalletDto;
            }

            return actionResult.Value;
        }
    }
}
