using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Internal;

public class InternalTourService : IInternalTourService
{
    private readonly ITourExecutionRepository _tourExecutionRepository;

    public InternalTourService(ITourExecutionRepository tourExecutionRepository)
    {
        _tourExecutionRepository = tourExecutionRepository;
    }

    public int GetCompletedToursCountForUser(long personId)
    {
        // personId = touristId in TourExecution table
        var executions = _tourExecutionRepository.GetAll(personId);
        return executions.Count(e => e.Status == TourExecutionStatus.completed);
    }
}
