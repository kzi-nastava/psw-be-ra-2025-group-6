using Explorer.Payments.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using Xunit;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class WalletServiceTests : BasePaymentsIntegrationTest
    {
        private const long TOURIST_ID = -21;

        public WalletServiceTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Top_up_succeeds()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var initialBalance = walletService.GetByTouristId(TOURIST_ID).BalanceAc;

            // Act
            var result = walletService.TopUp(TOURIST_ID, 100);

            // Assert
            result.ShouldNotBeNull();
            result.BalanceAc.ShouldBe(initialBalance + 100);
        }

        [Fact]
        public void Payment_succeeds()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
            walletService.TopUp(TOURIST_ID, 200);
            var initialBalance = walletService.GetByTouristId(TOURIST_ID).BalanceAc;

            // Act
            var result = walletService.Pay(TOURIST_ID, 50);

            // Assert
            result.ShouldNotBeNull();
            result.BalanceAc.ShouldBe(initialBalance - 50);
        }

        [Fact]
        public void Payment_fails_insufficient_funds()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
            var initialBalance = walletService.GetByTouristId(TOURIST_ID).BalanceAc;
            if(initialBalance < 100)
            {
                walletService.TopUp(TOURIST_ID, 100 - initialBalance);
            }
            initialBalance = walletService.GetByTouristId(TOURIST_ID).BalanceAc;


            // Act & Assert
            Should.Throw<ArgumentException>(() => walletService.Pay(TOURIST_ID, initialBalance + 50));
            var finalBalance = walletService.GetByTouristId(TOURIST_ID).BalanceAc;
            finalBalance.ShouldBe(initialBalance);
        }
    }
}
