using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourProblemRepository
{
    Task<List<TourProblem>> GetByTourist(long touristId);
    Task<TourProblem?> GetById(long id);
    Task<TourProblem> Create(TourProblem problem);
    Task<TourProblem> Update(TourProblem problem);
    Task Delete(long id);
}