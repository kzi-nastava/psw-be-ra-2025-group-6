using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests
{
    public class BaseEncountersIntegrationTest : BaseWebIntegrationTest<EncountersTestFactory>
    {
        public BaseEncountersIntegrationTest(EncountersTestFactory factory) : base(factory)
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EncountersContext>();
            ReseedDatabase(context);
        }

        private static void ReseedDatabase(EncountersContext context)
        {
            context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS encounters;");
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
            if (Directory.Exists(scriptFolder))
            {
                var scriptFiles = Directory.GetFiles(scriptFolder, "*.sql");
                Array.Sort(scriptFiles);
                var script = string.Join('\n', scriptFiles.Select(File.ReadAllText));
                context.Database.ExecuteSqlRaw(script);
            }
        }
    }
}
