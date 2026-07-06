using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class BuddyRepository : IBuddyRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public BuddyRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<BuddyDto>> SearchAsync(BuddySearchQuery query)
    {
        const string sql = """
            SELECT
                BuddyId, DisplayName, Age, LocalityId, Languages, VerificationLevel,
                Rating, CompletedTrips, Bio, ActivityTypes, AvailableToday,
                PreferredVenueIds, Gender, AvatarUrl
            FROM dbo.Buddies
            WHERE (@LocalityId IS NULL OR @LocalityId = '' OR LocalityId = @LocalityId)
            ORDER BY Rating DESC, CompletedTrips DESC
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<BuddyRow>(sql, new { query.localityId });

        var results = rows.Select(BuddyMapper.ToDto).ToList();

        if (!string.IsNullOrWhiteSpace(query.activityType))
        {
            results = results
                .Where(b => b.activityTypes.Contains(query.activityType, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        if (query.verifiedOnly)
        {
            results = results
                .Where(b => !string.Equals(b.verificationLevel, "Basic", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.venueId))
        {
            results = results
                .Where(b => b.preferredVenueIds.Contains(query.venueId, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        return results;
    }

    public async Task<BuddyDto?> GetByIdAsync(string buddyId)
    {
        const string sql = """
            SELECT
                BuddyId, DisplayName, Age, LocalityId, Languages, VerificationLevel,
                Rating, CompletedTrips, Bio, ActivityTypes, AvailableToday,
                PreferredVenueIds, Gender, AvatarUrl
            FROM dbo.Buddies
            WHERE BuddyId = @BuddyId
            """;

        using var connection = _connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<BuddyRow>(sql, new { BuddyId = buddyId });
        return row is null ? null : BuddyMapper.ToDto(row);
    }
}
