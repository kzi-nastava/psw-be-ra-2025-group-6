using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.TourProblem
{
    public interface ITourProblemService
    {
        Task<TourProblemDto> Create(TourProblemDto problemDto);
        Task<List<TourProblemDto>> GetByTourist(long touristId);
        Task<TourProblemDto> Update(TourProblemDto problemDto);
        Task Delete(long id, long touristId);
    }
}