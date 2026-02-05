using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests
{
    public class EncountersTestFactory: BaseTestFactory<EncountersContext>
    {
        static EncountersTestFactory()
        {
            NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
        }

        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EncountersContext>));
            services.Remove(descriptor);
            services.AddDbContext<EncountersContext>(SetupTestContext());

            // Register mock TourStatusGateway that always returns "CONFIRMED" for tests
            services.AddSingleton<ITourStatusGateway>(new MockTourStatusGateway());

            return services;
        }
    }

    // Simple mock implementation - no external dependencies needed
    internal class MockTourStatusGateway : ITourStatusGateway
    {
        public Task<string?> GetTourStatusByKeyPointId(long keyPointId)
        {
            // In tests, all tours are considered published
            return Task.FromResult<string?>("CONFIRMED");
        }
    }
}
