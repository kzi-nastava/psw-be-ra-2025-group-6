using System.IO;
using System.Linq;
using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
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

        // 1. Get all contexts
        var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var paymentsContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // 2. Reseed all databases
        ReseedPayments(paymentsContext);
        ReseedStakeholders(stakeholdersContext);
        ReseedDatabase(toursContext);
    }

    private static void ReseedDatabase(ToursContext context)
    {
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
    
    private static void ReseedPayments(PaymentsContext context)
    {
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS payments;");
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
    }

    private static void ReseedStakeholders(StakeholdersContext context)
    {
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
