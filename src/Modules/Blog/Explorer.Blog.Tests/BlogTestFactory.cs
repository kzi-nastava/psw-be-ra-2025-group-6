using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Explorer.Blog.Tests;

public class BlogTestFactory : BaseTestFactory<BlogContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BlogContext>));
        services.Remove(descriptor!);
        services.AddDbContext<BlogContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        return services;
    }

    protected override void SeedAdditionalDatabases(IServiceProvider services, string scriptFolder, ILogger logger)
    {
        var stakeholders = services.GetRequiredService<StakeholdersContext>();
        try
        {
            stakeholders.Database.EnsureCreated();
            var databaseCreator = stakeholders.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch (Exception)
        {
            // CreateTables throws if schema already exists.
        }

        try
        {
            var script = @"
INSERT INTO stakeholders.""Users""(""Id"", ""Username"", ""Password"", ""Role"", ""IsActive"")
VALUES (-11, 'autor1@gmail.com', 'autor1', 'Author', true)
ON CONFLICT DO NOTHING;

INSERT INTO stakeholders.""Users""(""Id"", ""Username"", ""Password"", ""Role"", ""IsActive"")
VALUES (-12, 'autor2@gmail.com', 'autor2', 'Author', true)
ON CONFLICT DO NOTHING;

INSERT INTO stakeholders.""Users""(""Id"", ""Username"", ""Password"", ""Role"", ""IsActive"")
VALUES (-21, 'turista1@gmail.com', 'turista1', 'Tourist', true)
ON CONFLICT DO NOTHING;
";
            stakeholders.Database.ExecuteSqlRaw(script);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding stakeholders for blog tests. Error: {Message}", ex.Message);
        }
    }
}
