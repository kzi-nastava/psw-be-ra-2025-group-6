using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.UseCases;
using Npgsql;

namespace Explorer.Stakeholders.Infrastructure.Integration
{
    public class TourInfoGateway : ITourInfoGateway
    {
        private readonly NpgsqlDataSource _dataSource;

        public TourInfoGateway()
        {
            var builder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("tours"));
            builder.EnableDynamicJson();
            _dataSource = builder.Build();
        }

        public async Task<TourInfo?> GetById(long id, CancellationToken cancellationToken = default)
        {
            const string query = "SELECT \"Id\", \"AuthorId\", \"Name\" FROM tours.\"Tours\" WHERE \"Id\" = @Id LIMIT 1;";
            try
            {
                await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@Id", id);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    return new TourInfo(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2));
                }

                return null;
            }
            catch (PostgresException ex) when (IsMissingSchema(ex))
            {
                return null;
            }
        }

        public async Task<List<TourInfo>> GetByAuthor(long authorId, CancellationToken cancellationToken = default)
        {
            const string query = "SELECT \"Id\", \"AuthorId\", \"Name\" FROM tours.\"Tours\" WHERE \"AuthorId\" = @AuthorId;";
            var tours = new List<TourInfo>();
            try
            {
                await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@AuthorId", authorId);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    tours.Add(new TourInfo(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2)));
                }

                return tours;
            }
            catch (PostgresException ex) when (IsMissingSchema(ex))
            {
                return tours;
            }
        }

        public async Task SuspendTour(long id, CancellationToken cancellationToken = default)
        {
            const string query = "UPDATE tours.\"Tours\" SET \"Status\" = @Status WHERE \"Id\" = @Id;";
            try
            {
                await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
                await using var command = connection.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Status", 3); // TourStatus.SUSPENDED
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (PostgresException ex) when (IsMissingSchema(ex))
            {
                // Ignore if tours schema not present in this environment
            }
        }

        private static bool IsMissingSchema(PostgresException ex) =>
            ex.SqlState == PostgresErrorCodes.InvalidSchemaName ||
            ex.SqlState == PostgresErrorCodes.InvalidCatalogName ||
            ex.SqlState == PostgresErrorCodes.UndefinedTable;
    }
}
