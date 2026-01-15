using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IProfilePostRepository : ICrudRepository<ProfilePost>
{
    List<ProfilePost> GetByAuthor(long authorId);
}
