using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface ITourPublishNotificationService
{
    void NotifyTourPublished(TourDto tour);
}
