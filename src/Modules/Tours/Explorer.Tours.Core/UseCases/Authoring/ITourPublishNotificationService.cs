using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Authoring;

public interface ITourPublishNotificationService
{
    void NotifyTourPublished(Tour tour);
}
