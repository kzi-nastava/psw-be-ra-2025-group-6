using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.UseCases;
using Npgsql;

namespace Explorer.Stakeholders.Infrastructure.Integration;

public class BlogInfoGateway : IBlogInfoGateway
{
    private readonly NpgsqlDataSource _dataSource;

    public BlogInfoGateway()
    {
        var builder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("blog"));
        builder.EnableDynamicJson();
        _dataSource = builder.Build();
    }

    public async Task<BlogInfo?> GetById(long id, CancellationToken cancellationToken = default)
    {
        const string query = "SELECT \"Id\", \"UserId\", \"Title\" FROM blog.\"Blogs\" WHERE \"Id\" = @Id LIMIT 1;";
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return new BlogInfo(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2));
            }
        }
        catch (PostgresException ex) when (IsMissingSchema(ex))
        {
            return null;
        }

        return null;
    }

    public async Task<List<BlogInfo>> GetByAuthor(long authorId, CancellationToken cancellationToken = default)
    {
        const string query = "SELECT \"Id\", \"UserId\", \"Title\" FROM blog.\"Blogs\" WHERE \"UserId\" = @AuthorId;";
        var blogs = new List<BlogInfo>();
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@AuthorId", authorId);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                blogs.Add(new BlogInfo(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2)));
            }
        }
        catch (PostgresException ex) when (IsMissingSchema(ex))
        {
            return blogs;
        }

        return blogs;
    }

    private static bool IsMissingSchema(PostgresException ex) =>
        ex.SqlState == PostgresErrorCodes.InvalidSchemaName ||
        ex.SqlState == PostgresErrorCodes.InvalidCatalogName ||
        ex.SqlState == PostgresErrorCodes.UndefinedTable;
}
