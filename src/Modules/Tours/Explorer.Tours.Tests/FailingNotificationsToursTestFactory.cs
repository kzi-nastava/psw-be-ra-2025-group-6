using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Tours.Tests;

public class FailingNotificationsToursTestFactory : ToursTestFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITourPublishNotificationService));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddScoped<ITourPublishNotificationService, FailingTourPublishNotificationService>();
        });
    }
}

public class FailingTourPublishNotificationService : ITourPublishNotificationService
{
    public void NotifyTourPublished(TourDto tour)
    {
        throw new InvalidOperationException("Simulated notification failure.");
    }
}
