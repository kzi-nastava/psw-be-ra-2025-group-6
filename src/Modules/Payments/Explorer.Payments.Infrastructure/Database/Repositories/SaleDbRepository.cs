using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class SaleDbRepository : ISaleRepository
{
    private readonly PaymentsContext _context;

    public SaleDbRepository(PaymentsContext context)
    {
        _context = context;
    }

    public Sale Create(Sale sale)
    {
        _context.Sales.Add(sale);
        _context.SaveChanges();
        return sale;
    }

    public Sale Update(Sale sale)
    {
        _context.Sales.Update(sale);
        _context.SaveChanges();
        return sale;
    }

    public void Delete(long id)
    {
        var sale = Get(id);
        _context.Sales.Remove(sale);
        _context.SaveChanges();
    }

    public Sale Get(long id)
    {
        var sale = _context.Sales.Find(id);
        if (sale == null)
            throw new NotFoundException($"Sale with id {id} not found");
        return sale;
    }

    public List<Sale> GetByAuthor(long authorId)
    {
        return _context.Sales
            .Where(s => s.AuthorId == authorId)
            .ToList();
    }

    public List<Sale> GetActiveSales()
    {
        var now = DateTime.UtcNow;
        return _context.Sales
            .Where(s => s.StartDate <= now && s.EndDate >= now)
            .ToList();
    }

    public Sale? GetActiveSaleForTour(long tourId)
    {
        var now = DateTime.UtcNow;
        return _context.Sales
            .Where(s => s.StartDate <= now && s.EndDate >= now)
            .ToList()
            .FirstOrDefault(s => s.TourIds.Contains(tourId));
    }
}
