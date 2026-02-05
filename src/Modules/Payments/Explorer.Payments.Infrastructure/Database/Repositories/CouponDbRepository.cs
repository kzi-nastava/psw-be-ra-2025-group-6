using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class CouponDbRepository : ICouponRepository
{
    private readonly PaymentsContext _context;

    public CouponDbRepository(PaymentsContext context)
    {
        _context = context;
    }

    public Coupon Create(Coupon coupon)
    {
        _context.Coupons.Add(coupon);
        _context.SaveChanges();
        return coupon;
    }

    public Coupon Update(Coupon coupon)
    {
        _context.Coupons.Update(coupon);
        _context.SaveChanges();
        return coupon;
    }

    public void Delete(long id)
    {
        var coupon = Get(id);
        _context.Coupons.Remove(coupon);
        _context.SaveChanges();
    }

    public Coupon Get(long id)
    {
        var coupon = _context.Coupons.Find(id);
        if (coupon == null)
            throw new NotFoundException($"Coupon with id {id} not found");
        return coupon;
    }

    public Coupon? GetByCode(string code)
    {
        return _context.Coupons.FirstOrDefault(c => c.Code == code);
    }

    public List<Coupon> GetByAuthor(long authorId)
    {
        return _context.Coupons
            .Where(c => c.AuthorId == authorId)
            .ToList();
    }
}
