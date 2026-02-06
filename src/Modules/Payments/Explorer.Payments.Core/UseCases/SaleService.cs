using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public SaleService(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public SaleDto Create(long authorId, CreateSaleDto dto)
    {
        var sale = new Sale(authorId, dto.TourIds, dto.StartDate, dto.EndDate, dto.DiscountPercent);
        var result = _saleRepository.Create(sale);
        return _mapper.Map<SaleDto>(result);
    }

    public SaleDto Update(long authorId, long saleId, CreateSaleDto dto)
    {
        var existingSale = _saleRepository.Get(saleId);

        if (existingSale.AuthorId != authorId)
            throw new ForbiddenException("You can only update your own sales");

        var updatedSale = new Sale(authorId, dto.TourIds, dto.StartDate, dto.EndDate, dto.DiscountPercent);
        var result = _saleRepository.Update(updatedSale);
        return _mapper.Map<SaleDto>(result);
    }

    public void Delete(long authorId, long saleId)
    {
        var sale = _saleRepository.Get(saleId);

        if (sale.AuthorId != authorId)
            throw new ForbiddenException("You can only delete your own sales");

        _saleRepository.Delete(saleId);
    }

    public SaleDto Get(long id)
    {
        var sale = _saleRepository.Get(id);
        return _mapper.Map<SaleDto>(sale);
    }

    public List<SaleDto> GetByAuthor(long authorId)
    {
        var sales = _saleRepository.GetByAuthor(authorId);
        return _mapper.Map<List<SaleDto>>(sales);
    }

    public List<SaleDto> GetActiveSales()
    {
        var sales = _saleRepository.GetActiveSales();
        return _mapper.Map<List<SaleDto>>(sales);
    }
}
