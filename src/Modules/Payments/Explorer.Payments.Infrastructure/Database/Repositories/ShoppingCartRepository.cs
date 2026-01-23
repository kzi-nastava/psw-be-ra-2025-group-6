using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly PaymentsContext _dbContext;
    private readonly DbSet<ShoppingCart> _dbSet;

    public ShoppingCartRepository(PaymentsContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<ShoppingCart>();
    }

    public ShoppingCart? GetByTouristId(long touristId)
    {
        return _dbSet
            .Include(s => s.Items)
            .Include(s => s.BundleItems)  // ? Dodato
            .FirstOrDefault(s => s.TouristId == touristId);
    }

    public ShoppingCart Create(ShoppingCart cart)
    {
        _dbSet.Add(cart);
        _dbContext.SaveChanges();
        return cart;
    }

    public ShoppingCart Update(ShoppingCart cart)
    {
        try
        {
            _dbContext.Update(cart);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return cart;
    }

    public void Delete(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
