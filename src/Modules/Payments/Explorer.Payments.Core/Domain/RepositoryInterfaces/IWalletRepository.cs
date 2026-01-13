using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface IWalletRepository
{
    Wallet? GetByTouristId(long touristId);
    Wallet Create(Wallet wallet);
    Wallet Update(Wallet wallet);
}
