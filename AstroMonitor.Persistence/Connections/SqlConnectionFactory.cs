using System.Data;
using AstroMonitor.Application.Common.Interfaces;
using Npgsql;

namespace AstroMonitor.Persistence.Connections;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;
    
    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }
    
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}