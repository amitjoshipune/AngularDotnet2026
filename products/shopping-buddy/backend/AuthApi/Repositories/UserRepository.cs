using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class UserRepository : IUserRepository
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
        var user = await connection.QuerySingleOrDefaultAsync<UserRecord>(sql, new { Email = email.Trim() });
        if (user is null)
        {
            return null;
        }

        user.Roles = (await GetRolesForUserAsync(connection, user.UserId)).ToList();
        if (user.Roles.Count == 0)
        {
            user.Roles.Add(user.Role);
        }

        return user;
    }

    public async Task<IReadOnlyList<string>> GetRolesForUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await GetRolesForUserAsync(connection, userId);
    }

    private static async Task<IReadOnlyList<string>> GetRolesForUserAsync(System.Data.IDbConnection connection, int userId)
    {
        const string rolesSql = """
            SELECT Role FROM dbo.UserRoles WHERE UserId = @UserId
            UNION
            SELECT Role FROM dbo.Users WHERE UserId = @UserId
              AND NOT EXISTS (SELECT 1 FROM dbo.UserRoles WHERE UserId = @UserId)
            """;

        var roles = await connection.QueryAsync<string>(rolesSql, new { UserId = userId });
        return roles.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}
