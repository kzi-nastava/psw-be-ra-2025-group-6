using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class PaymentRecordDbRepository : IPaymentRecordRepository
{
    private readonly PaymentsContext _context;

    public PaymentRecordDbRepository(PaymentsContext context)
    {
        _context = context;
    }

    public PaymentRecord Create(PaymentRecord record)
    {
        _context.PaymentRecords.Add(record);
        _context.SaveChanges();
        return record;
    }

    public PaymentRecord Get(long id)
    {
        var record = _context.PaymentRecords.Find(id);
        if (record == null)
            throw new NotFoundException($"Payment record with id {id} not found");
        return record;
    }

    public List<PaymentRecord> GetByTourist(long touristId)
    {
        return _context.PaymentRecords
            .Where(r => r.TouristId == touristId)
            .ToList();
    }
}
