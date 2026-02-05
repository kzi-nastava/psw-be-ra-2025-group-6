using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface IWalletService
{
    WalletDto GetByTouristId(long touristId);
    WalletDto CreateForTourist(long touristId);
    WalletDto TopUp(long touristId, double amount);
    WalletDto Pay(long touristId, double amount);
}
