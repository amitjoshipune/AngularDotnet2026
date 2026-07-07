namespace AuthApi.Models;

public class LocalityDto
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string zone { get; set; } = string.Empty;
}

public class VenueDto
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string localityId { get; set; } = string.Empty;
    public string type { get; set; } = string.Empty;
}

public class BuddyDto
{
    public string id { get; set; } = string.Empty;
    public string displayName { get; set; } = string.Empty;
    public int age { get; set; }
    public string localityId { get; set; } = string.Empty;
    public string[] languages { get; set; } = Array.Empty<string>();
    public string verificationLevel { get; set; } = string.Empty;
    public decimal rating { get; set; }
    public int completedTrips { get; set; }
    public string bio { get; set; } = string.Empty;
    public string[] activityTypes { get; set; } = Array.Empty<string>();
    public bool availableToday { get; set; }
    public string[] preferredVenueIds { get; set; } = Array.Empty<string>();
    public string gender { get; set; } = "Female";
    public string? avatarUrl { get; set; }
}

public class BuddySearchQuery
{
    public string? localityId { get; set; }
    public string? venueId { get; set; }
    public string? activityType { get; set; }
    public bool verifiedOnly { get; set; }
}

public class CreateBookingRequest
{
    public string buddyId { get; set; } = string.Empty;
    public string venueId { get; set; } = string.Empty;
    public string date { get; set; } = string.Empty;
    public string timeSlot { get; set; } = string.Empty;
    public string activityType { get; set; } = string.Empty;
    public string? notes { get; set; }
    public bool acceptedSafetyRules { get; set; }
    public bool shareLiveLocation { get; set; }
}

public class BookingConfirmationDto
{
    public string bookingId { get; set; } = string.Empty;
    public string buddyName { get; set; } = string.Empty;
    public string venueName { get; set; } = string.Empty;
    public string localityName { get; set; } = string.Empty;
    public string date { get; set; } = string.Empty;
    public string timeSlot { get; set; } = string.Empty;
    public string safetyPin { get; set; } = string.Empty;
    public string status { get; set; } = "PendingBuddy";
    public string emergencyContact { get; set; } = "+91 98XX-XXX-XXX (demo)";
}

public class BookingListItemDto
{
    public string bookingId { get; set; } = string.Empty;
    public string buddyName { get; set; } = string.Empty;
    public string customerName { get; set; } = string.Empty;
    public string venueName { get; set; } = string.Empty;
    public string localityName { get; set; } = string.Empty;
    public string date { get; set; } = string.Empty;
    public string timeSlot { get; set; } = string.Empty;
    public string activityType { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public string safetyPin { get; set; } = string.Empty;
    public string? specialNotes { get; set; }
    public string? rejectionReasonCode { get; set; }
    public string? rejectionReasonText { get; set; }
}

public class BuddyIncomingBookingDto
{
    public string bookingId { get; set; } = string.Empty;
    public string customerName { get; set; } = string.Empty;
    public string customerEmail { get; set; } = string.Empty;
    public string venueName { get; set; } = string.Empty;
    public string localityName { get; set; } = string.Empty;
    public string date { get; set; } = string.Empty;
    public string timeSlot { get; set; } = string.Empty;
    public string activityType { get; set; } = string.Empty;
    public string? specialNotes { get; set; }
    public string status { get; set; } = string.Empty;
    public string createdAt { get; set; } = string.Empty;
}

public class RejectBookingRequest
{
    public string reasonCode { get; set; } = string.Empty;
    public string? reasonText { get; set; }
}

public class RejectionReasonDto
{
    public string code { get; set; } = string.Empty;
    public string label { get; set; } = string.Empty;
    public bool requiresText { get; set; }
}

internal class BuddyRow
{
    public string BuddyId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string LocalityId { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;
    public string VerificationLevel { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int CompletedTrips { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string ActivityTypes { get; set; } = string.Empty;
    public bool AvailableToday { get; set; }
    public string PreferredVenueIds { get; set; } = string.Empty;
    public string Gender { get; set; } = "Female";
    public string? AvatarUrl { get; set; }
}

internal static class BuddyMapper
{
    public static BuddyDto ToDto(BuddyRow row) => new()
    {
        id = row.BuddyId,
        displayName = row.DisplayName,
        age = row.Age,
        localityId = row.LocalityId,
        languages = SplitCsv(row.Languages),
        verificationLevel = row.VerificationLevel,
        rating = row.Rating,
        completedTrips = row.CompletedTrips,
        bio = row.Bio,
        activityTypes = SplitCsv(row.ActivityTypes),
        availableToday = row.AvailableToday,
        preferredVenueIds = SplitCsv(row.PreferredVenueIds),
        gender = row.Gender,
        avatarUrl = row.AvatarUrl
    };

    private static string[] SplitCsv(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
