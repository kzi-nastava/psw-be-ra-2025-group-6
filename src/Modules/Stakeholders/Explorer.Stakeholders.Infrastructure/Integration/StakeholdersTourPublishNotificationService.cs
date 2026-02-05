using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.UseCases.Authoring;
using Explorer.Tours.Core.UseCases.Tourist;

namespace Explorer.Stakeholders.Infrastructure.Integration;

public class StakeholdersTourPublishNotificationService : ITourPublishNotificationService
{
    private const string NewTourTitle = "New tour published";
    private readonly ITouristPreferencesRepository _preferencesRepository;
    private readonly INotificationRepository _notificationRepository;

    public StakeholdersTourPublishNotificationService(
        ITouristPreferencesRepository preferencesRepository,
        INotificationRepository notificationRepository)
    {
        _preferencesRepository = preferencesRepository;
        _notificationRepository = notificationRepository;
    }

    public void NotifyTourPublished(Tour tour)
    {
        var preferences = _preferencesRepository.GetAll();
        foreach (var preference in preferences)
        {
            var snapshot = new TouristPreferencesSnapshot
            {
                PreferredDifficulty = preference.PreferredDifficulty,
                WalkRating = preference.WalkRating,
                BikeRating = preference.BikeRating,
                CarRating = preference.CarRating,
                BoatRating = preference.BoatRating,
                Tags = preference.Tags
            };

            if (TourRecommendationScoring.CalculateScore(tour, snapshot) < TourRecommendationScoring.RecommendationThreshold)
            {
                continue;
            }

            if (_notificationRepository.ExistsForRecipientAndReference(preference.TouristId, tour.Id, NewTourTitle))
            {
                continue;
            }

            var message = $"New tour \"{tour.Name}\" matches your preferences.";
            var notification = new Notification(preference.TouristId, tour.AuthorId, message, tour.Id, NewTourTitle);
            _notificationRepository.Create(notification);
        }
    }
}
