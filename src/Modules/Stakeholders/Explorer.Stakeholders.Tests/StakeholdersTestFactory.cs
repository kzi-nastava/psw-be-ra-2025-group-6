using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var paymentsContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<StakeholdersTestFactory>>();

            try
            {
                paymentsContext.Database.EnsureCreated();
                var databaseCreator = paymentsContext.Database.GetService<IRelationalDatabaseCreator>();
                databaseCreator.CreateTables();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred initializing the payments database for stakeholder tests. Error: {Message}", ex.Message);
            }
        });
    }

    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
         var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
         services.Remove(descriptor!);
         services.AddDbContext<StakeholdersContext>(SetupTestContext());

         var paymentsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
         if (paymentsDescriptor != null)
         {
             services.Remove(paymentsDescriptor);
             services.AddDbContext<PaymentsContext>(SetupTestContext());
         }

         return services;
        
    }
}
