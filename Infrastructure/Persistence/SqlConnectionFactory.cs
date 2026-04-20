using System.Data;
using Application.Abstractions.Data;
using Npgsql;

namespace Infrastructure.Persistence;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
