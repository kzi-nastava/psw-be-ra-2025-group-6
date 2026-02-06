namespace Explorer.Tours.API.Internal;

public interface IInternalTourService
{
    // PersonId = TouristId u TourExecution tabeli
    int GetCompletedToursCountForUser(long personId);
}
