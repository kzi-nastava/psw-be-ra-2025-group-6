using System.IO;
using System.Linq;
using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Tours.Tests;

public class BaseToursIntegrationTest : BaseWebIntegrationTest<ToursTestFactory>
{
    public BaseToursIntegrationTest(ToursTestFactory factory) : base(factory)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        ReseedDatabase(db);

        // Seed authentication-related data needed for Tours integration tests
        var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        ReseedStakeholders(stakeholdersContext);
    }

    private static void ReseedDatabase(ToursContext context)
    {
        // Tests rely on EF model being the source of truth. Since we use EnsureCreated/CreateTables
        // (not migrations), we need a clean schema each run to avoid stale columns.
        context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS tours CASCADE;");
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS tours;");
        context.Database.EnsureCreated();
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch
        {
            // Tables already exist
        }

        var scriptFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestData"));
        var scriptFiles = Directory.GetFiles(scriptFolder);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        context.Database.ExecuteSqlRaw(script);
    }

    private static void ReseedStakeholders(StakeholdersContext context)
    {
        context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS stakeholders CASCADE;");
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS stakeholders;");
        context.Database.EnsureCreated();
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch
        {
            // Tables already exist
        }

        var scriptFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Stakeholders", "Explorer.Stakeholders.Tests", "TestData"));
        var scriptFiles = Directory.GetFiles(scriptFolder);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        context.Database.ExecuteSqlRaw(script);
    }
}
