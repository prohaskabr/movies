using Npgsql;
using System.Data;

namespace Movies.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(token);
        return conn;
    }
}