using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.UseCases;
using Explorer.Tours.Core.UseCases.Authoring;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Tours.Core.Domain;
using Npgsql;
using Explorer.Tours.API.Public.TourProblem;

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
        services.AddScoped<ITouristEquipmentService, TouristEquipmentService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IMonumentService, MonumentService>();
        services.AddScoped<IMeetupService, MeetupService>();
        services.AddScoped<ITourProblemService, TourProblemService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IEquipmentRepository, EquipmentDbRepository>();
        services.AddScoped<ITouristEquipmentRepository, TouristEquipmentDbRepository>();
        services.AddScoped<ITourRepository<Tour>, TourRepository<Tour,ToursContext>>();
        services.AddScoped<IMonumentRepository, MonumentDbRepository>();
        services.AddScoped<IMeetupRepository, MeetupRepository>();
        services.AddScoped<ITourProblemRepository, TourProblemRepository>();


        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("tours"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        services.AddDbContext<ToursContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "tours")));
    }
}
