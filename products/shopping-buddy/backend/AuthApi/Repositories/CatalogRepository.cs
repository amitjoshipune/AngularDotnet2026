using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class CatalogRepository : ICatalogRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CatalogRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<LocalityDto>> GetLocalitiesAsync()
    {
        const string sql = """
            SELECT LocalityId AS id, Name AS name, Zone AS zone
            FROM dbo.Localities
            ORDER BY Name
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<LocalityDto>(sql);
        return rows.ToList();
    }

    public async Task<IReadOnlyList<VenueDto>> GetVenuesAsync(string? localityId)
    {
        const string sql = """
            SELECT VenueId AS id, Name AS name, LocalityId AS localityId, VenueType AS type
            FROM dbo.Venues
            WHERE (@LocalityId IS NULL OR LocalityId = @LocalityId)
            ORDER BY Name
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<VenueDto>(sql, new { LocalityId = localityId });
        return rows.ToList();
    }

    public async Task<string?> GetLocalityNameAsync(string localityId)
    {
        const string sql = "SELECT Name FROM dbo.Localities WHERE LocalityId = @LocalityId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<string?>(sql, new { LocalityId = localityId });
    }
}
