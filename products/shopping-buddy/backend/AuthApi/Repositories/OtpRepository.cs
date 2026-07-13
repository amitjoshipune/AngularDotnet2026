using AuthApi.Data;
using Dapper;

namespace AuthApi.Repositories;

public sealed class OtpRepository : IOtpRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public OtpRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(string email, string code, string purpose, DateTime expiresAtUtc)
    {
        const string sql = """
            INSERT INTO dbo.OtpChallenges (Email, Code, Purpose, ExpiresAt)
            VALUES (@Email, @Code, @Purpose, @ExpiresAt)
            """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Email = email.Trim(),
            Code = code,
            Purpose = purpose,
            ExpiresAt = expiresAtUtc
        });
    }

    public async Task<bool> TryConsumeAsync(string email, string code, string purpose)
    {
        const string sql = """
            SELECT TOP (1) OtpChallengeId
            FROM dbo.OtpChallenges
            WHERE Email = @Email
              AND Code = @Code
              AND Purpose = @Purpose
              AND UsedAt IS NULL
              AND ExpiresAt > SYSUTCDATETIME()
            ORDER BY CreatedAt DESC
            """;

        using var connection = _connectionFactory.CreateConnection();
        var challengeId = await connection.QuerySingleOrDefaultAsync<int?>(sql, new
        {
            Email = email.Trim(),
            Code = code.Trim(),
            Purpose = purpose
        });

        if (challengeId is null)
        {
            return false;
        }

        const string markUsedSql = """
            UPDATE dbo.OtpChallenges
            SET UsedAt = SYSUTCDATETIME()
            WHERE OtpChallengeId = @OtpChallengeId AND UsedAt IS NULL
            """;

        var rows = await connection.ExecuteAsync(markUsedSql, new { OtpChallengeId = challengeId.Value });
        return rows == 1;
    }
}
