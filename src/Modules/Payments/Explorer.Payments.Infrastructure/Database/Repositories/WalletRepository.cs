using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly PaymentsContext _dbContext;
    private readonly DbSet<Wallet> _dbSet;

    public WalletRepository(PaymentsContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Wallet>();
    }

    public Wallet? GetByTouristId(long touristId)
    {
        return _dbSet.FirstOrDefault(w => w.TouristId == touristId);
    }

    public Wallet Create(Wallet wallet)
    {
        _dbSet.Add(wallet);
        _dbContext.SaveChanges();
        return wallet;
    }

    public Wallet Update(Wallet wallet)
    {
        try
        {
            _dbContext.Update(wallet);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }

        return wallet;
    }
}
