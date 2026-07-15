namespace AuthApi.Models;

public static class DocumentTypes
{
    public const string Aadhaar = "Aadhaar";
    public const string AddressProof = "AddressProof";
    public const string Pan = "PAN";

    public static readonly string[] RequiredForBuddyAccept = [Aadhaar, AddressProof];

    /** MVP: ID upload is optional; gate disabled until legal review. */
    public static readonly bool EnforceBuddyDocumentGate = false;
}

public static class DocumentStatus
{
    public const string Pending = "Pending";
    public const string Verified = "Verified";
    public const string Rejected = "Rejected";
}

public class UserAddressDto
{
    public int addressId { get; set; }
    public string label { get; set; } = string.Empty;
    public string line1 { get; set; } = string.Empty;
    public string? line2 { get; set; }
    public string city { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
    public string pincode { get; set; } = string.Empty;
    public bool isDefault { get; set; }
}

public class UserMeDto
{
    public string id { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string displayName { get; set; } = string.Empty;
    public List<string> roles { get; set; } = new();
    public string? phone { get; set; }
    public string? dateOfBirth { get; set; }
    public string? gender { get; set; }
    public string? emergencyContactName { get; set; }
    public string? emergencyContactPhone { get; set; }
    public string? bio { get; set; }
    public string? profilePhotoUrl { get; set; }
    public string buddyApplicationStatus { get; set; } = "None";
    public string? buddyApplicationNotes { get; set; }
    public string? buddyLocalityId { get; set; }
    public List<UserAddressDto> addresses { get; set; } = new();
}

public class UpdateUserMeRequest
{
    /** basic = name/phone/bio/photo only; advanced = emergency/address/dob; all = everything */
    public string updateScope { get; set; } = "all";
    public string displayName { get; set; } = string.Empty;
    public string? phone { get; set; }
    public string? dateOfBirth { get; set; }
    public string? gender { get; set; }
    public string? emergencyContactName { get; set; }
    public string? emergencyContactPhone { get; set; }
    public string? bio { get; set; }
    public string? profilePhotoUrl { get; set; }
    public List<UserAddressDto> addresses { get; set; } = new();
}

public class BuddyApplyRequest
{
    public string localityId { get; set; } = string.Empty;
    public string bio { get; set; } = string.Empty;
    public string? notes { get; set; }
}

public class UserDocumentDto
{
    public int documentId { get; set; }
    public string documentType { get; set; } = string.Empty;
    public string fileName { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public string uploadedAt { get; set; } = string.Empty;
    public string? verifiedAt { get; set; }
    public string? rejectionReason { get; set; }
}

public class VerificationStatusDto
{
    /** True when buddy may accept bookings. MVP: always true (ID optional). */
    public bool canAcceptBookings { get; set; } = true;
    public List<string> missingDocuments { get; set; } = new();
    public List<UserDocumentDto> documents { get; set; } = new();
    public string note { get; set; } = "ID documents are optional in this MVP build.";
}
