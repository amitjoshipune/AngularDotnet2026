using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class UserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UserRecord?> FindByEmailAsync(string email)
    {
        const string sql = """
            SELECT UserId, Email, Password, DisplayName, Role
            FROM dbo.Users
            WHERE Email = @Email AND IsActive = 1
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserRecord>(sql, new { Email = email.Trim() });
    }
}
