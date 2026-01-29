using Explorer.Blog.Infrastructure;
using Explorer.Stakeholders.Infrastructure;
using Explorer.Tours.Infrastructure;
using Explorer.Encounters.Infrastructure;
using Explorer.Payments.Infrastructure;
using Explorer.API.Dispatchers;
using Shared.Achievements;
using Shared;
using Explorer.Stakeholders.Core.UseCases;

namespace Explorer.API.Startup;

public static class ModulesConfiguration
{
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        services.ConfigureToursModule();
        services.ConfigureStakeholdersModule();
        services.ConfigureBlogModule();
        services.ConfigureEncountersModule();
        services.ConfigurePaymentsModule();

        services.AddScoped<
    IDomainEventHandler<AchievementUnlockedEvent>, AchievementUnlockedHandler>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();


        return services;
    }
}