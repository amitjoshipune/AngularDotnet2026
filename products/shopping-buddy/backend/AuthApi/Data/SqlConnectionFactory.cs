using Microsoft.Data.SqlClient;

namespace AuthApi.Data;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ShoppingBuddy")
            ?? throw new InvalidOperationException("Connection string 'ShoppingBuddy' is not configured.");
    }

    public SqlConnection CreateConnection() => new(_connectionString);
}
