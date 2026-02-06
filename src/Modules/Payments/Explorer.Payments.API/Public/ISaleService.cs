using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface ISaleService
{
    SaleDto Create(long authorId, CreateSaleDto dto);
    SaleDto Update(long authorId, long saleId, CreateSaleDto dto);
    void Delete(long authorId, long saleId);
    SaleDto Get(long id);
    List<SaleDto> GetByAuthor(long authorId);
    List<SaleDto> GetActiveSales();
}
