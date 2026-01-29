using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
    public interface ITourPlannerService
    {
        TourPlannerDto Create(long userId, TourPlannerCreateDto dto);
        TourPlannerDto Update(long id, long userId, TourPlannerUpdateDto dto);

        void Delete(long id);

        TourPlannerDto GetById(long id);
        List<TourPlannerDto> GetAllByUserId(long userId);

        PagedResult<TourPlannerDto> GetByUserId(long userId, int page, int size);
    }
}
