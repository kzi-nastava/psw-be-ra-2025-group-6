using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface IPaymentRecordRepository : ICrudRepository<PaymentRecord>
{
}
