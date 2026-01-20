using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface ISocialMessageService
    {
        Task<SocialMessageDto> Create(SocialMessageDto messageDto);
        Task<PagedResult<SocialMessageDto>> GetConversation(long userId1, long userId2, int page, int pageSize);
    }
}
