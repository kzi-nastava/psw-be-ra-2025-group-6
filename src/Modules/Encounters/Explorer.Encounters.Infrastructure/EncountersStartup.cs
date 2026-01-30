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

            services.AddDbContext<EncountersContext>(opt =>
                opt.UseNpgsql(DbConnectionStringBuilder.Build("encounters"),
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "encounters")));
        }
    }
}