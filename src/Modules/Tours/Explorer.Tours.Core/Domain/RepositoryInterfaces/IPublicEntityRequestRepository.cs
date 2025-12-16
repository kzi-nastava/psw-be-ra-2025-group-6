using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IPublicEntityRequestRepository
{
    PagedResult<PublicEntityRequest> GetPaged(int page, int pageSize);
    PublicEntityRequest Get(long id);
    PublicEntityRequest Create(PublicEntityRequest request);
    PublicEntityRequest Update(PublicEntityRequest request);
    List<PublicEntityRequest> GetByAuthor(long authorId);
    List<PublicEntityRequest> GetPending();
    List<PublicEntityRequest> GetAll();
}
