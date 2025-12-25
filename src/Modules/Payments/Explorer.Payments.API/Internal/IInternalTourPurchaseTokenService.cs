namespace Explorer.Payments.API.Internal;

public interface IInternalTourPurchaseTokenService
{
    //ovaj interfejs sam napravio jer je pucao test u architecture zbog komunikacije Core -> Core, pa sam nekako morao da uklonim tu zavisnost
    bool DoesTouristHaveUnusedToken(long touristId, long tourId);
    void MarkTokenAsUsed(long touristId, long tourId);
    List<long> GetPurchasedTourIds(long touristId);
}
