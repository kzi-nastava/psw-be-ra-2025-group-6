using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface ITourProblemRepository
{
    Task<List<TourProblem>> GetByTourist(long touristId);
    Task<List<TourProblem>> GetByTourIds(List<long> tourIds);
    Task<TourProblem?> GetById(long id);
    Task<TourProblem> Create(TourProblem problem);
    Task<TourProblem> Update(TourProblem problem);
    Task Delete(long id);
}