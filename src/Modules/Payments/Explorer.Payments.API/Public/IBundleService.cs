using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface IBundleService
{
    BundleDto Create(long authorId, CreateBundleDto dto);
    BundleDto Update(long authorId, long bundleId, UpdateBundleDto dto);
    void Delete(long authorId, long bundleId);
    BundleDto Get(long id);
    List<BundleDto> GetByAuthor(long authorId);
    List<BundleDto> GetPublished();
    BundleDto Publish(long authorId, long bundleId);
    BundleDto Archive(long authorId, long bundleId);
    double GetTotalToursPrice(List<long> tourIds);
}
