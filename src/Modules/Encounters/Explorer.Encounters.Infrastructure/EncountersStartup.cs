using Explorer.Encounters.Core.Mappers;
using Explorer.Encounters.Infrastructure.Database; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Infrastructure.Database.Repositories;
using Explorer.Encounters.Core.UseCases;
using AutoMapper;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Internal;
using Explorer.Encounters.Infrastructure.Integration;
using Npgsql;

namespace Explorer.Encounters.Infrastructure
{
    public static class EncountersStartup
    {
        public static IServiceCollection ConfigureEncountersModule(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(EncountersProfile).Assembly);
            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            services.AddScoped<IChallengeService, ChallengeService>();
            services.AddScoped<Explorer.Encounters.API.Public.IChallengePublicService, PublicChallengeService>();
            services.AddScoped<ITouristEncounterService, TouristEncounterService>();

            services.AddScoped<ISocialEncounterService, SocialEncounterService>();

            services.AddScoped<IHiddenLocationService, HiddenLocationService>();
            

            services.AddScoped<ILeaderboardService, LeaderboardService>();
            services.AddScoped<IInternalLeaderboardService, InternalLeaderboardService>();
            services.AddScoped<ILeaderboardNotificationService, LeaderboardNotificationService>();

            services.AddScoped<IQuizEncounterService, QuizEncounterService>();

        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            services.AddScoped<IChallengeRepository, ChallengeDbRepository>();
            services.AddScoped<ITouristXpProfileRepository, TouristXpProfileDbRepository>();
            services.AddScoped<IEncounterCompletionRepository, EncounterCompletionDbRepository>();

            services.AddScoped<ISocialEncounterRepository, SocialEncounterDatabaseRepository>();
            services.AddScoped<IActiveSocialParticipantRepository, ActiveSocialParticipantDatabaseRepository>();

            services.AddScoped<IHiddenLocationAttemptRepository, HiddenLocationAttemptDbRepository>();
            
            services.AddScoped<ILeaderboardEntryRepository, LeaderboardEntryDbRepository>();
            services.AddScoped<IClubLeaderboardRepository, ClubLeaderboardDbRepository>();


            services.AddScoped<IQuizEncounterRepository, QuizEncounterDbRepository>();
            services.AddScoped<IQuizCompletionRepository, QuizCompletionDbRepository>();

            // Register TourStatusGateway for cross-module communication
            services.AddScoped<Core.Domain.RepositoryInterfaces.ITourStatusGateway>(provider => 
                new TourStatusGateway(DbConnectionStringBuilder.Build("tours")));

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("encounters"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<EncountersContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "encounters")));
        }
    }
}
