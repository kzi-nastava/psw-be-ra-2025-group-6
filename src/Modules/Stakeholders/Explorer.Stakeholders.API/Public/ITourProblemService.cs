using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITourProblemService
    {
        Task<TourProblemDto> Create(TourProblemDto problemDto);
        Task<List<TourProblemDto>> GetByTourist(long touristId);
        Task<TourProblemDto> Update(TourProblemDto problemDto);
        Task Delete(long id, long touristId);
    }
}