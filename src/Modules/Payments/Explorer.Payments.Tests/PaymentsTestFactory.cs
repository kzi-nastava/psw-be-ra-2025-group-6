using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Payments.Tests;

public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        // Replace main DbContext
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
            services.AddDbContext<PaymentsContext>(SetupTestContext());
        }

        // Replace other DbContexts that tests might depend on
        var toursDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        if (toursDescriptor != null)
        {
            services.Remove(toursDescriptor);
            services.AddDbContext<ToursContext>(SetupTestContext());
        }

        var stakeholdersDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        if (stakeholdersDescriptor != null)
        {
            services.Remove(stakeholdersDescriptor);
            services.AddDbContext<StakeholdersContext>(SetupTestContext());
        }
        
        return services;
    }
}
