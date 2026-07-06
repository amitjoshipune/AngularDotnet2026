using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class BookingRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public BookingRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<BookingConfirmationDto?> CreateAsync(int customerUserId, CreateBookingRequest request)
    {
        const string buddySql = "SELECT DisplayName FROM dbo.Buddies WHERE BuddyId = @BuddyId";
        const string venueSql = """
            SELECT v.Name AS VenueName, l.Name AS LocalityName
            FROM dbo.Venues v
            INNER JOIN dbo.Localities l ON l.LocalityId = v.LocalityId
            WHERE v.VenueId = @VenueId
            """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var buddyName = await connection.QuerySingleOrDefaultAsync<string?>(buddySql, new { request.buddyId });
        var venue = await connection.QuerySingleOrDefaultAsync<VenueInfoRow>(
            venueSql, new { request.venueId });

        if (buddyName is null || venue is null)
        {
            return null;
        }

        if (!DateOnly.TryParse(request.date, out var bookingDate))
        {
            return null;
        }

        var bookingId = $"BMSB-{DateTime.UtcNow:yyMMddHHmmss}";
        var safetyPin = Random.Shared.Next(1000, 9999).ToString();

        const string insertSql = """
            INSERT INTO dbo.Bookings
            (
                BookingId, CustomerUserId, BuddyId, VenueId, ActivityType, BookingDate,
                TimeSlot, SpecialNotes, SafetyPin, ShareLiveLocation, Status
            )
            VALUES
            (
                @BookingId, @CustomerUserId, @BuddyId, @VenueId, @ActivityType, @BookingDate,
                @TimeSlot, @Notes, @SafetyPin, @ShareLiveLocation, N'Confirmed'
            )
            """;

        await connection.ExecuteAsync(insertSql, new
        {
            BookingId = bookingId,
            CustomerUserId = customerUserId,
            request.buddyId,
            request.venueId,
            request.activityType,
            BookingDate = bookingDate.ToDateTime(TimeOnly.MinValue),
            request.timeSlot,
            Notes = request.notes,
            SafetyPin = safetyPin,
            request.shareLiveLocation
        });

        return new BookingConfirmationDto
        {
            bookingId = bookingId,
            buddyName = buddyName,
            venueName = venue.VenueName,
            localityName = venue.LocalityName,
            date = bookingDate.ToString("yyyy-MM-dd"),
            timeSlot = request.timeSlot,
            safetyPin = safetyPin
        };
    }

    public async Task<IReadOnlyList<BookingListItemDto>> GetForCustomerAsync(int customerUserId)
    {
        const string sql = """
            SELECT
                b.BookingId AS bookingId,
                buddy.DisplayName AS buddyName,
                v.Name AS venueName,
                l.Name AS localityName,
                CONVERT(varchar(10), b.BookingDate, 23) AS date,
                b.TimeSlot AS timeSlot,
                b.ActivityType AS activityType,
                b.Status AS status,
                b.SafetyPin AS safetyPin
            FROM dbo.Bookings b
            INNER JOIN dbo.Buddies buddy ON buddy.BuddyId = b.BuddyId
            INNER JOIN dbo.Venues v ON v.VenueId = b.VenueId
            INNER JOIN dbo.Localities l ON l.LocalityId = v.LocalityId
            WHERE b.CustomerUserId = @CustomerUserId
            ORDER BY b.BookingDate DESC, b.CreatedAt DESC
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<BookingListItemDto>(sql, new { CustomerUserId = customerUserId });
        return rows.ToList();
    }
}

internal sealed class VenueInfoRow
{
    public string VenueName { get; set; } = string.Empty;
    public string LocalityName { get; set; } = string.Empty;
}
