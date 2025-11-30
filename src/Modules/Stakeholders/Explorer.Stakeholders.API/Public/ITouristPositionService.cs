using Explorer.Stakeholders.API.Dtos;
namespace Explorer.Stakeholders.API.Public;

public interface ITouristPositionService
{
    TouristPositionDto CreateOrUpdate(TouristPositionDto touristPosition);
}
