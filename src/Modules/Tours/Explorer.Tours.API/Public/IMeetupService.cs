using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface IMeetupService
{
    PagedResult<MeetupDto> GetPaged(int page, int pageSize);
    MeetupDto Get(long id);
    MeetupDto Create(MeetupDto meetup);
    MeetupDto Update(MeetupDto meetup);
    void Delete(long id);
}
