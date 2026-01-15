using Explorer.Payments.Core.Domain;


namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface IPaymentRecordRepository
{
    PaymentRecord Create(PaymentRecord record);
    PaymentRecord Get(long id);
    List<PaymentRecord> GetByTourist(long touristId);
}
