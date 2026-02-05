using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface ISaleRepository
{
    Sale Create(Sale sale);
    Sale Update(Sale sale);
    void Delete(long id);
    Sale Get(long id);
    List<Sale> GetByAuthor(long authorId);
    List<Sale> GetActiveSales();
    Sale? GetActiveSaleForTour(long tourId);
}
