using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        ShoppingCart GetByTouristId(long touristId);
        ShoppingCart Update(ShoppingCart cart);
        ShoppingCart Create(ShoppingCart cart);
    }
}
