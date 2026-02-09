using Explorer.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Explorer.BuildingBlocks.Tests;

public abstract class BaseTestFactory<TDbContext> : WebApplicationFactory<Program> where TDbContext : DbContext
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            using var scope = BuildServiceProvider(services).CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<BaseTestFactory<TDbContext>>>();

            // Resolve TestData relative to the test project's output directory so it works regardless of content root
            var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestData"));
            InitializeDatabase(db, path, logger);
            SeedAdditionalDatabases(scopedServices, path, logger);
        });
    }

    private static void InitializeDatabase(DbContext context, string scriptFolder, ILogger logger)
    {
        try
        {
            // First, ensure the database exists
            EnsureDatabaseExists(logger);
            
            var defaultSchema = context.Model.GetDefaultSchema();
            if (!string.IsNullOrWhiteSpace(defaultSchema))
            {
                context.Database.ExecuteSqlRaw($"CREATE SCHEMA IF NOT EXISTS \"{defaultSchema}\";");
            }

            context.Database.EnsureCreated();
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch (Exception)
        {
            // CreateTables throws an exception if the schema already exists. This is a workaround for multiple dbcontexts.
        }

        try
        {
            var scriptFiles = Directory.GetFiles(scriptFolder);
            Array.Sort(scriptFiles);
            var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
            context.Database.ExecuteSqlRaw(script);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the database with test data. Error: {Message}", ex.Message);
        }
    }

    private static void EnsureDatabaseExists(ILogger logger)
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "explorer-v1-test";
        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "root";

        // Connect to postgres database to create the test database if it doesn't exist
        var masterConnectionString = $"Server={server};Port={port};Database=postgres;User ID={user};Password={password};";
        
        try
        {
            using var connection = new NpgsqlConnection(masterConnectionString);
            connection.Open();
            
            // Check if database exists
            using var checkCmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{database}'", connection);
            var exists = checkCmd.ExecuteScalar() != null;
            
            if (!exists)
            {
                logger.LogInformation("Creating test database: {Database}", database);
                using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{database}\"", connection);
                createCmd.ExecuteNonQuery();
                logger.LogInformation("Test database created successfully: {Database}", database);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not ensure database exists: {Message}", ex.Message);
        }
    }

    private ServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return ReplaceNeededDbContexts(services).BuildServiceProvider();
    }

    protected abstract IServiceCollection ReplaceNeededDbContexts(IServiceCollection services);

    protected virtual void SeedAdditionalDatabases(IServiceProvider services, string scriptFolder, ILogger logger)
    {
    }

    protected static Action<DbContextOptionsBuilder> SetupTestContext()
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "explorer-v1-test";
        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "root";
        var pooling = Environment.GetEnvironmentVariable("DATABASE_POOLING") ?? "true";

        var connectionString = $"Server={server};Port={port};Database={database};User ID={user};Password={password};Pooling={pooling};Include Error Detail=True";

        return opt => opt.UseNpgsql(connectionString);
    }
}
