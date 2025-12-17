using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IFollowerService
    {
        List<FollowerDto> GetAllFollowers(long userId);
    }
}
