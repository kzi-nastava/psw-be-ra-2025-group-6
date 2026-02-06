using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class BundleDbRepository : IBundleRepository
{
    private readonly PaymentsContext _context;

    public BundleDbRepository(PaymentsContext context)
    {
        _context = context;
    }

    public Bundle Create(Bundle bundle)
    {
        _context.Bundles.Add(bundle);
        _context.SaveChanges();
        return bundle;
    }

    public Bundle Update(Bundle bundle)
    {
        _context.Bundles.Update(bundle);
        _context.SaveChanges();
        return bundle;
    }

    public void Delete(long id)
    {
        var bundle = Get(id);
        _context.Bundles.Remove(bundle);
        _context.SaveChanges();
    }

    public Bundle Get(long id)
    {
        var bundle = _context.Bundles.Find(id);
        if (bundle == null)
            throw new NotFoundException($"Bundle with id {id} not found");
        return bundle;
    }

    public List<Bundle> GetByAuthor(long authorId)
    {
        return _context.Bundles
            .Where(b => b.AuthorId == authorId)
            .ToList();
    }

    public List<Bundle> GetPublished()
    {
        return _context.Bundles
            .Where(b => b.Status == BundleStatus.PUBLISHED)
            .ToList();
    }
}
