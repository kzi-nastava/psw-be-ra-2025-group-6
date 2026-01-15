using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface IBundleRepository
{
    Bundle Create(Bundle bundle);
    Bundle Update(Bundle bundle);
    void Delete(long id);
    Bundle Get(long id);
    List<Bundle> GetByAuthor(long authorId);
    List<Bundle> GetPublished();
}
