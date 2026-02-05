using Explorer.API.Middleware;
using Explorer.API.Startup;
using System.Data;
using System.Data.Common;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Search;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(builder.Configuration);
const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();

builder.Services.RegisterModules();

builder.Services.AddScoped<ISearchService, SearchService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
    MigrateWithBaselineIfNeeded(stakeholdersDb, "stakeholders", "20260102064851_Init", "Users");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Explorer API v1");
});

if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseRouting();
app.UseCors(corsPolicy);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();

static void MigrateWithBaselineIfNeeded(DbContext db, string schema, string baselineMigrationId, string sentinelTable)
{
    var connection = db.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open) connection.Open();

    var history = db.Database.GetService<IHistoryRepository>();
    var historyExists = TableExists(connection, schema, "__EFMigrationsHistory");

    if (!historyExists)
    {
        db.Database.ExecuteSqlRaw(history.GetCreateScript());
    }

    var historyHasRows = TableHasAnyRows(connection, schema, "__EFMigrationsHistory");
    var hasSentinelTable = TableExists(connection, schema, sentinelTable);

    if (!historyHasRows && hasSentinelTable)
    {
        var productVersion = db.Model.GetProductVersion() ?? "8.0.0";
        db.Database.ExecuteSqlRaw(history.GetInsertScript(new HistoryRow(baselineMigrationId, productVersion)));
    }

    db.Database.Migrate();
}

static bool TableExists(DbConnection connection, string schema, string table)
{
    using var cmd = connection.CreateCommand();
    cmd.CommandText = "SELECT to_regclass(@fullName) IS NOT NULL;";
    var fullName = $"{schema}.\"{table}\"";
    var param = cmd.CreateParameter();
    param.ParameterName = "fullName";
    param.Value = fullName;
    cmd.Parameters.Add(param);
    return cmd.ExecuteScalar() as bool? == true;
}

static bool TableHasAnyRows(DbConnection connection, string schema, string table)
{
    using var cmd = connection.CreateCommand();
    cmd.CommandText = $"SELECT EXISTS (SELECT 1 FROM \"{schema}\".\"{table}\" LIMIT 1);";
    return cmd.ExecuteScalar() as bool? == true;
}

// Required for automated tests
namespace Explorer.API
{
    public partial class Program { }
}
