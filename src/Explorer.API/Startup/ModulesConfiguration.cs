using Explorer.Blog.Infrastructure;
using Explorer.Stakeholders.Infrastructure;
using Explorer.Tours.Infrastructure;
using Explorer.Encounters.Infrastructure;


namespace Explorer.API.Startup;

public static class ModulesConfiguration
{
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        services.ConfigureToursModule();
        services.ConfigureStakeholdersModule();
        services.ConfigureBlogModule();
        services.ConfigureEncountersModule();
        return services;
    }
}