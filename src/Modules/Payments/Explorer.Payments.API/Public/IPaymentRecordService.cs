using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface IPaymentRecordService
{
    PaymentRecordDto Create(PaymentRecordDto paymentRecord);
}
