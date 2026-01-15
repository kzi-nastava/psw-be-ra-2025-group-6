using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class PaymentRecordService : IPaymentRecordService
    {
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly IMapper _mapper;

        public PaymentRecordService(IPaymentRecordRepository paymentRecordRepository, IMapper mapper)
        {
            _paymentRecordRepository = paymentRecordRepository;
            _mapper = mapper;
        }

        public PaymentRecordDto Create(PaymentRecordDto paymentRecordDto)
        {
            var paymentRecord = _mapper.Map<PaymentRecord>(paymentRecordDto);
            var result = _paymentRecordRepository.Create(paymentRecord);
            return _mapper.Map<PaymentRecordDto>(result);
        }
    }
}
