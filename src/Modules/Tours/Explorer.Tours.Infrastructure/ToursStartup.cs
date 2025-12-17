using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Admin;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases;
using Explorer.Tours.Core.UseCases.Admin;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.UseCases.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Explorer.Tours.Core.UseCases.Tourist;

namespace Explorer.Tours.Infrastructure;

public static class ToursStartup
{
    public static IServiceCollection ConfigureToursModule(this IServiceCollection services)
    {
        // Registers all profiles since it works on the assembly
        services.AddAutoMapper(typeof(ToursProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IJournalService, JournalService>();
        services.AddScoped<IAnnualAwardService, AnnualAwardService>();
        services.AddScoped<ITouristEquipmentService, TouristEquipmentService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IMonumentService, MonumentService>();
        services.AddScoped<IMeetupService, MeetupService>();
        services.AddScoped<Explorer.Tours.API.Public.Execution.ITourExecutionService, Explorer.Tours.Core.UseCases.Execution.TourExecutionService>();
        services.AddScoped<IAdminMapService, AdminMapService>();
        services.AddScoped<ITourReviewService, TourReviewService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();

        services.AddScoped<ITouristViewService, TouristViewService>();
        services.AddScoped<IQuizService, QuizService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IEquipmentRepository, EquipmentDbRepository>();
        services.AddScoped<IFacilityRepository, FacilityDbRepository>();
        services.AddScoped<IJournalRepository, JournalDbRepository>();
        services.AddScoped<IAnnualAwardRepository<AnnualAward>, AnnualAwardRepository<AnnualAward, ToursContext>>();
        services.AddScoped<ITouristEquipmentRepository, TouristEquipmentDbRepository>();
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<IMonumentRepository, MonumentDbRepository>();
        services.AddScoped<IMeetupRepository, MeetupRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>();
        services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();


        // Repo for executions
        services.AddScoped<Core.Domain.RepositoryInterfaces.ITourExecutionRepository, Tours.Infrastructure.Database.Repositories.TourExecutionDbRepository>();

        services.AddScoped<IQuizRepository, QuizDbRepository>();


        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("tours"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        services.AddDbContext<ToursContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "tours")));
    }
}