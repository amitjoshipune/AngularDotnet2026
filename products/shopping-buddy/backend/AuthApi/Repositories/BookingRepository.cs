using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private static readonly HashSet<string> ValidReasonCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "ScheduleConflict", "VenueTooFar", "PersonalEmergency", "SafetyConcern", "Other"
    };

    private readonly ISqlConnectionFactory _connectionFactory;

    public BookingRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<(BookingConfirmationDto? confirmation, bool isDuplicate)> CreateAsync(
        int customerUserId,
        CreateBookingRequest request)
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
            return (null, false);
        }

        if (!DateOnly.TryParse(request.date, out var bookingDate))
        {
            return (null, false);
        }

        const string duplicateSql = """
            SELECT COUNT(1)
            FROM dbo.Bookings
            WHERE CustomerUserId = @CustomerUserId
              AND VenueId = @VenueId
              AND BookingDate = @BookingDate
              AND TimeSlot = @TimeSlot
              AND Status NOT IN (N'Cancelled', N'RejectedByBuddy')
            """;

        var duplicateCount = await connection.ExecuteScalarAsync<int>(duplicateSql, new
        {
            CustomerUserId = customerUserId,
            request.venueId,
            BookingDate = bookingDate.ToDateTime(TimeOnly.MinValue),
            request.timeSlot
        });

        if (duplicateCount > 0)
        {
            return (null, true);
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
                @TimeSlot, @Notes, @SafetyPin, @ShareLiveLocation, N'PendingBuddy'
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

        return (new BookingConfirmationDto
        {
            bookingId = bookingId,
            buddyName = buddyName,
            venueName = venue.VenueName,
            localityName = venue.LocalityName,
            date = bookingDate.ToString("yyyy-MM-dd"),
            timeSlot = request.timeSlot,
            safetyPin = safetyPin,
            status = "PendingBuddy"
        }, false);
    }

    public async Task<IReadOnlyList<BookingListItemDto>> GetForCustomerAsync(int customerUserId)
    {
        const string sql = """
            SELECT
                b.BookingId AS bookingId,
                buddy.DisplayName AS buddyName,
                c.DisplayName AS customerName,
                v.Name AS venueName,
                l.Name AS localityName,
                CONVERT(varchar(10), b.BookingDate, 23) AS date,
                b.TimeSlot AS timeSlot,
                b.ActivityType AS activityType,
                b.Status AS status,
                b.SafetyPin AS safetyPin,
                b.SpecialNotes AS specialNotes,
                b.RejectionReasonCode AS rejectionReasonCode,
                b.RejectionReasonText AS rejectionReasonText
            FROM dbo.Bookings b
            INNER JOIN dbo.Buddies buddy ON buddy.BuddyId = b.BuddyId
            INNER JOIN dbo.Venues v ON v.VenueId = b.VenueId
            INNER JOIN dbo.Localities l ON l.LocalityId = v.LocalityId
            INNER JOIN dbo.Users c ON c.UserId = b.CustomerUserId
            WHERE b.CustomerUserId = @CustomerUserId
            ORDER BY b.BookingDate DESC, b.CreatedAt DESC
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<BookingListItemDto>(sql, new { CustomerUserId = customerUserId });
        return rows.ToList();
    }

    public async Task<IReadOnlyList<BuddyIncomingBookingDto>> GetIncomingForBuddyUserAsync(int buddyUserId)
    {
        const string sql = """
            SELECT
                b.BookingId AS bookingId,
                c.DisplayName AS customerName,
                c.Email AS customerEmail,
                v.Name AS venueName,
                l.Name AS localityName,
                CONVERT(varchar(10), b.BookingDate, 23) AS date,
                b.TimeSlot AS timeSlot,
                b.ActivityType AS activityType,
                b.SpecialNotes AS specialNotes,
                b.Status AS status,
                CONVERT(varchar(19), b.CreatedAt, 126) AS createdAt
            FROM dbo.Bookings b
            INNER JOIN dbo.Buddies buddy ON buddy.BuddyId = b.BuddyId
            INNER JOIN dbo.Users c ON c.UserId = b.CustomerUserId
            INNER JOIN dbo.Venues v ON v.VenueId = b.VenueId
            INNER JOIN dbo.Localities l ON l.LocalityId = v.LocalityId
            WHERE buddy.UserId = @BuddyUserId
            ORDER BY
                CASE WHEN b.Status = N'PendingBuddy' THEN 0 ELSE 1 END,
                b.BookingDate DESC,
                b.CreatedAt DESC
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<BuddyIncomingBookingDto>(sql, new { BuddyUserId = buddyUserId });
        return rows.ToList();
    }

    public async Task<string?> GetBuddyIdForUserAsync(int userId)
    {
        const string sql = "SELECT BuddyId FROM dbo.Buddies WHERE UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<string?>(sql, new { UserId = userId });
    }

    public async Task<bool> ConfirmAsync(string bookingId, int buddyUserId)
    {
        const string sql = """
            UPDATE b
            SET Status = N'Confirmed',
                BuddyRespondedAt = SYSUTCDATETIME(),
                RejectionReasonCode = NULL,
                RejectionReasonText = NULL
            FROM dbo.Bookings b
            INNER JOIN dbo.Buddies buddy ON buddy.BuddyId = b.BuddyId
            WHERE b.BookingId = @BookingId
              AND buddy.UserId = @BuddyUserId
              AND b.Status = N'PendingBuddy'
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { BookingId = bookingId, BuddyUserId = buddyUserId });
        return rows > 0;
    }

    public async Task<bool> RejectAsync(string bookingId, int buddyUserId, RejectBookingRequest request)
    {
        if (!ValidReasonCodes.Contains(request.reasonCode))
        {
            return false;
        }

        if (string.Equals(request.reasonCode, "Other", StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(request.reasonText))
        {
            return false;
        }

        const string sql = """
            UPDATE b
            SET Status = N'RejectedByBuddy',
                BuddyRespondedAt = SYSUTCDATETIME(),
                RejectionReasonCode = @ReasonCode,
                RejectionReasonText = @ReasonText
            FROM dbo.Bookings b
            INNER JOIN dbo.Buddies buddy ON buddy.BuddyId = b.BuddyId
            WHERE b.BookingId = @BookingId
              AND buddy.UserId = @BuddyUserId
              AND b.Status = N'PendingBuddy'
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            BookingId = bookingId,
            BuddyUserId = buddyUserId,
            ReasonCode = request.reasonCode,
            ReasonText = request.reasonText
        });
        return rows > 0;
    }
}

internal sealed class VenueInfoRow
{
    public string VenueName { get; set; } = string.Empty;
    public string LocalityName { get; set; } = string.Empty;
}
