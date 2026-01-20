using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ISocialMessageRepository : ICrudRepository<SocialMessage>
    {
        PagedResult<SocialMessage> GetConversation(long userId1, long userId2, int page, int pageSize);
    }
}