using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface IShoppingCartService
{
    ShoppingCartDto GetByTouristId(long touristId);
    ShoppingCartDto AddItem(long touristId, long tourId, string tourName, double price);
    ShoppingCartDto RemoveItem(long touristId, long tourId);
    List<TourPurchaseTokenDto> Checkout(long touristId);
    List<TourPurchaseTokenDto> CheckoutWithCoupon(long touristId, string couponCode);
    PaymentRecordDto PurchaseBundle(long touristId, long bundleId);
}
