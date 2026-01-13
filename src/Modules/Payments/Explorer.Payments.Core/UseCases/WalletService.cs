using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public WalletService(IWalletRepository walletRepository, IMapper mapper)
    {
        _walletRepository = walletRepository;
        _mapper = mapper;
    }

    public WalletDto GetByTouristId(long touristId)
    {
        var wallet = _walletRepository.GetByTouristId(touristId);
        if (wallet == null)
        {
            wallet = _walletRepository.Create(new Wallet(touristId));
        }

        return _mapper.Map<WalletDto>(wallet);
    }

    public WalletDto CreateForTourist(long touristId)
    {
        var wallet = _walletRepository.GetByTouristId(touristId);
        if (wallet == null)
        {
            wallet = _walletRepository.Create(new Wallet(touristId));
        }

        return _mapper.Map<WalletDto>(wallet);
    }

    public WalletDto TopUp(long touristId, double amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");

        var wallet = _walletRepository.GetByTouristId(touristId);
        if (wallet == null)
        {
            wallet = new Wallet(touristId);
            wallet.IncreaseBalance(amount);
            wallet = _walletRepository.Create(wallet);
        }
        else
        {
            wallet.IncreaseBalance(amount);
            wallet = _walletRepository.Update(wallet);
        }

        return _mapper.Map<WalletDto>(wallet);
    }
}
