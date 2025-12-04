using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITourProblemMessageService
    {
        Task<TourProblemMessageDto> Create(TourProblemMessageDto messageDto);
        Task<PagedResult<TourProblemMessageDto>> GetForProblem(long problemId, int page, int pageSize);
    }
}
