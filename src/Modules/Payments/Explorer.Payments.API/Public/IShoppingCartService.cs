using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface IShoppingCartService
{
    ShoppingCartDto GetByTouristId(long touristId);
    ShoppingCartDto AddItem(long touristId, long tourId, string tourName, double price);
    ShoppingCartDto AddBundle(long touristId, long bundleId);
    ShoppingCartDto RemoveItem(long touristId, long tourId);
    ShoppingCartDto RemoveBundle(long touristId, long bundleId);
    List<TourPurchaseTokenDto> Checkout(long touristId);
    List<TourPurchaseTokenDto> CheckoutWithCoupon(long touristId, string couponCode);
    PaymentRecordDto PurchaseBundle(long touristId, long bundleId);
    CheckoutPreviewDto GetCheckoutPreview(long touristId);
    CheckoutPreviewDto GetCheckoutPreviewWithCoupon(long touristId, string couponCode);
}
