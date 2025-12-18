using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IReviewAppService
    {
        ReviewAppDto Create(CreateReviewAppDto dto, long userId);
        ReviewAppDto Update(long reviewId, UpdateReviewAppDto dto, long userId);
        List<ReviewAppDto> GetAll();
        List<ReviewAppDto> GetByUser(long userId);
        PagedResult<ReviewAppDto> GetPaged(int page, int pageSize);

        void Delete(long reviewId, long userId);
    }
}
