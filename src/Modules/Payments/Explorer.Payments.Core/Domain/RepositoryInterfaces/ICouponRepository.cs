using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface ICouponRepository
{
    Coupon Create(Coupon coupon);
    Coupon Update(Coupon coupon);
    void Delete(long id);
    Coupon Get(long id);
    Coupon? GetByCode(string code);
    List<Coupon> GetByAuthor(long authorId);
}
