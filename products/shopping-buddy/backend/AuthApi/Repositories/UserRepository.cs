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
            SELECT UserId, Email, Password, DisplayName, Role, EmailVerified
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

    public async Task<UserRecord?> FindByDisplayNameAsync(string displayName)
    {
        const string sql = """
            SELECT TOP (1) UserId, Email, Password, DisplayName, Role, EmailVerified
            FROM dbo.Users
            WHERE DisplayName = @DisplayName AND IsActive = 1
            ORDER BY UserId
            """;

        using var connection = _connectionFactory.CreateConnection();
        var user = await connection.QuerySingleOrDefaultAsync<UserRecord>(sql, new { DisplayName = displayName.Trim() });
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

    public async Task<UserRecord?> FindByIdAsync(int userId)
    {
        const string sql = """
            SELECT UserId, Email, Password, DisplayName, Role, EmailVerified
            FROM dbo.Users
            WHERE UserId = @UserId AND IsActive = 1
            """;

        using var connection = _connectionFactory.CreateConnection();
        var user = await connection.QuerySingleOrDefaultAsync<UserRecord>(sql, new { UserId = userId });
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

    public async Task UpdateDisplayNameAsync(
        int userId,
        string displayName,
        System.Data.IDbConnection connection,
        System.Data.IDbTransaction transaction)
    {
        const string sql = """
            UPDATE dbo.Users SET DisplayName = @DisplayName WHERE UserId = @UserId
            """;

        await connection.ExecuteAsync(sql, new { UserId = userId, DisplayName = displayName }, transaction);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        const string sql = """
            SELECT 1 FROM dbo.Users WHERE Email = @Email
            """;

        using var connection = _connectionFactory.CreateConnection();
        var exists = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { Email = email.Trim() });
        return exists.HasValue;
    }

    public async Task<int> CreateCustomerAsync(string email, string passwordHash, string displayName)
    {
        const string insertUserSql = """
            INSERT INTO dbo.Users (Email, Password, DisplayName, Role, EmailVerified)
            OUTPUT INSERTED.UserId
            VALUES (@Email, @Password, @DisplayName, N'Customer', 0)
            """;

        using var connection = _connectionFactory.CreateConnection();
        var userId = await connection.QuerySingleAsync<int>(insertUserSql, new
        {
            Email = email.Trim(),
            Password = passwordHash,
            DisplayName = displayName.Trim()
        });

        const string insertRoleSql = """
            IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NOT NULL
            BEGIN
                INSERT INTO dbo.UserRoles (UserId, Role)
                SELECT @UserId, N'Customer'
                WHERE NOT EXISTS (
                    SELECT 1 FROM dbo.UserRoles WHERE UserId = @UserId AND Role = N'Customer'
                );
            END
            """;

        await connection.ExecuteAsync(insertRoleSql, new { UserId = userId });
        return userId;
    }

    public async Task SetEmailVerifiedAsync(int userId)
    {
        const string sql = """
            UPDATE dbo.Users
            SET EmailVerified = 1,
                EmailVerifiedAt = SYSUTCDATETIME()
            WHERE UserId = @UserId
            """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task UpdatePasswordAsync(int userId, string passwordHash)
    {
        const string sql = """
            UPDATE dbo.Users
            SET Password = @Password
            WHERE UserId = @UserId
            """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId, Password = passwordHash });
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
