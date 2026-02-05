using Npgsql;
using System.Threading.Tasks;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Infrastructure.Integration
{
    public class TourStatusGateway : ITourStatusGateway
    {
        private readonly string _connectionString;

        public TourStatusGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string?> GetTourStatusByKeyPointId(long keyPointId)
        {
            const string query = @"
                SELECT t.""Status"" 
                FROM tours.""Tours"" t
                INNER JOIN tours.""KeyPoints"" kp ON t.""Id"" = kp.""TourId""
                WHERE kp.""Id"" = @KeyPointId
                LIMIT 1;
            ";

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@KeyPointId", keyPointId);

                var result = await command.ExecuteScalarAsync();
                if (result == null) return null;

                // Map enum value to string
                // 0 = DRAFT, 1 = CONFIRMED, 2 = ARCHIVED, 3 = SUSPENDED
                var statusValue = result.ToString();
                return statusValue switch
                {
                    "0" => "DRAFT",
                    "1" => "CONFIRMED",
                    "2" => "ARCHIVED",
                    "3" => "SUSPENDED",
                    _ => statusValue // Fallback if already a string
                };
            }
            catch (PostgresException ex) when (IsMissingSchema(ex))
            {
                // Tours schema not present - return null
                return null;
            }
        }

        private static bool IsMissingSchema(PostgresException ex) =>
            ex.SqlState == PostgresErrorCodes.InvalidSchemaName ||
            ex.SqlState == PostgresErrorCodes.InvalidCatalogName ||
            ex.SqlState == PostgresErrorCodes.UndefinedTable;
    }
}
