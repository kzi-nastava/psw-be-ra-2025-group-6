using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface ITouristPositionRepository
{
    TouristPosition CreateOrUpdate(TouristPosition position);
    TouristPosition? GetByTouristId(long touristId);
}
