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
        private const long MISSING_TOURIST_ID = -9999;

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
        public void Top_up_rejects_zero_amount()
        {
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            Should.Throw<ArgumentException>(() => walletService.TopUp(TOURIST_ID, 0));
        }

        [Fact]
        public void Top_up_rejects_negative_amount()
        {
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            Should.Throw<ArgumentException>(() => walletService.TopUp(TOURIST_ID, -10));
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
        public void Payment_rejects_zero_amount()
        {
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            Should.Throw<ArgumentException>(() => walletService.Pay(TOURIST_ID, 0));
        }

        [Fact]
        public void Payment_rejects_negative_amount()
        {
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            Should.Throw<ArgumentException>(() => walletService.Pay(TOURIST_ID, -5));
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

        [Fact]
        public void Payment_fails_when_wallet_missing()
        {
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            Should.Throw<InvalidOperationException>(() => walletService.Pay(MISSING_TOURIST_ID, 10));
        }

        [Fact]
        public void Create_for_tourist_returns_existing_wallet()
        {
            using var scope = Factory.Services.CreateScope();
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            var existing = walletService.GetByTouristId(TOURIST_ID);
            var created = walletService.CreateForTourist(TOURIST_ID);

            created.Id.ShouldBe(existing.Id);
            created.BalanceAc.ShouldBe(existing.BalanceAc);
        }
    }
}
