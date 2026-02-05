using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
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
            
            // Also ensure Stakeholders Notifications table has Type column
            var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            EnsureNotificationsTypeColumn(stakeholdersContext);
        }

        private static void ReseedDatabase(EncountersContext context)
        {
            context.Database.ExecuteSqlRaw("DROP SCHEMA IF EXISTS encounters CASCADE;");
            context.Database.ExecuteSqlRaw("CREATE SCHEMA IF NOT EXISTS encounters;");
            
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
        
        private static void EnsureNotificationsTypeColumn(StakeholdersContext context)
        {
            try
            {
                context.Database.ExecuteSqlRaw(@"
                    DO $$
                    BEGIN
                        IF NOT EXISTS (
                            SELECT 1 FROM information_schema.columns 
                            WHERE table_schema = 'stakeholders' 
                            AND table_name = 'Notifications' 
                            AND column_name = 'Type'
                        ) THEN
                            ALTER TABLE stakeholders.""Notifications"" ADD COLUMN ""Type"" integer NOT NULL DEFAULT 0;
                        END IF;
                    END $$;
                ");
            }
            catch
            {
                // Column already exists or table doesn't exist yet
            }
        }
    }
}
