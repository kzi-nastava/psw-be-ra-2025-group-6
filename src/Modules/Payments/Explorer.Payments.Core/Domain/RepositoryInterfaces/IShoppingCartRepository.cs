using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface IShoppingCartRepository
{
    ShoppingCart? GetByTouristId(long touristId);
    ShoppingCart Create(ShoppingCart cart);
    ShoppingCart Update(ShoppingCart cart);
    void Delete(long id);
}
