using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface IPublicEntityRequestService
{
    PagedResult<PublicEntityRequestDto> GetPaged(int page, int pageSize);
    PublicEntityRequestDto Get(long id);
    PublicEntityRequestDto CreateRequest(CreatePublicEntityRequestDto dto, long authorId);
    List<PublicEntityRequestDto> GetByAuthor(long authorId);
    List<PublicEntityRequestDto> GetPending();
    
    // ADMIN METHODS
    PublicEntityRequestDto ApproveRequest(long requestId, long adminId);
    PublicEntityRequestDto RejectRequest(long requestId, long adminId, string comment);
}
