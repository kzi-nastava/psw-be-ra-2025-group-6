using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IReviewAppService
    {
        ReviewAppDto Create(CreateReviewAppDto dto);
        ReviewAppDto Update(long id, UpdateReviewAppDto dto);
        List<ReviewAppDto> GetAll();
        List<ReviewAppDto> GetByUser(long userId);
        PagedResult<ReviewAppDto> GetPaged(int page, int pageSize);
    }
}
