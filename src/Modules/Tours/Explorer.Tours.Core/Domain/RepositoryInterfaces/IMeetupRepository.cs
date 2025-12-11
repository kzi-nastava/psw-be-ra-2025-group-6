using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IMeetupRepository
{
    PagedResult<Meetup> GetPaged(int page, int pageSize);
    Meetup Get(long id);
    Meetup Create(Meetup meetup);
    Meetup Update(Meetup meetup);
    void Delete(long id);
}
