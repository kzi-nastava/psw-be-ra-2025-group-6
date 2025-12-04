using Explorer.Tours.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.API.Public
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetByTouristId(long touristId);
        ShoppingCartDto AddItem(long touristId, long tourId);
        ShoppingCartDto RemoveItem(long touristId, long tourId);
    }
}
