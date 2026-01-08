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

namespace Explorer.Payments.Tests;

public class BasePaymentsIntegrationTest : BaseWebIntegrationTest<PaymentsTestFactory>
{
    public BasePaymentsIntegrationTest(PaymentsTestFactory factory) : base(factory)
    {
        using var scope = Factory.Services.CreateScope();

        // Get all contexts
        var paymentsContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Reseed all databases
        ReseedPayments(paymentsContext);
        ReseedStakeholders(stakeholdersContext);
        ReseedTours(toursContext);
    }
    
    private static void ReseedPayments(PaymentsContext context)
    {
        context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS payments CASCADE;");
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS payments;");
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch { /* Tables already exist */ }
    }

    private static void ReseedTours(ToursContext context)
    {
        context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS tours CASCADE;");
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS tours;");
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch { /* Tables already exist */ }

        // Seed data for Tours
        var scriptFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Tours", "Explorer.Tours.Tests", "TestData"));
        var scriptFiles = Directory.GetFiles(scriptFolder);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        context.Database.ExecuteSqlRaw(script);
    }

    private static void ReseedStakeholders(StakeholdersContext context)
    {
        context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS stakeholders CASCADE;");
        context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS stakeholders;");
        try
        {
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch { /* Tables already exist */ }

        // Seed data for Stakeholders
        var scriptFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Stakeholders", "Explorer.Stakeholders.Tests", "TestData"));
        var scriptFiles = Directory.GetFiles(scriptFolder);
        Array.Sort(scriptFiles);
        var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
        context.Database.ExecuteSqlRaw(script);
    }
}
