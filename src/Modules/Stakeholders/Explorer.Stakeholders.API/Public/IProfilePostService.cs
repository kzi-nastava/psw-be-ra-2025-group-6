using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IProfilePostService
{
    ProfilePostDto Create(ProfilePostDto dto);
    ProfilePostDto Update(ProfilePostDto dto);
    void Delete(long id, long authorId);
    List<ProfilePostDto> GetByAuthor(long authorId);
    PagedResult<ProfilePostDto> GetPagedByAuthor(long authorId, int page, int pageSize);
}
