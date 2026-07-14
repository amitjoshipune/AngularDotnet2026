using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IUserRepository _users;

    public UserProfileRepository(ISqlConnectionFactory connectionFactory, IUserRepository users)
    {
        _connectionFactory = connectionFactory;
        _users = users;
    }

    public async Task<UserMeDto?> GetMeAsync(int userId)
    {
        var user = await _users.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        using var connection = _connectionFactory.CreateConnection();

        const string profileSql = """
            SELECT Phone, DateOfBirth, Gender, EmergencyContactName, EmergencyContactPhone, Bio,
                   ProfilePhotoUrl, BuddyApplicationStatus, BuddyApplicationNotes, BuddyLocalityId
            FROM dbo.UserProfiles
            WHERE UserId = @UserId
            """;

        var profile = await connection.QuerySingleOrDefaultAsync<ProfileRow>(profileSql, new { UserId = userId });

        const string addressSql = """
            SELECT AddressId AS addressId, Label AS label, Line1 AS line1, Line2 AS line2,
                   City AS city, State AS state, Pincode AS pincode, IsDefault AS isDefault
            FROM dbo.UserAddresses
            WHERE UserId = @UserId
            ORDER BY IsDefault DESC, AddressId
            """;

        var addresses = (await connection.QueryAsync<UserAddressDto>(addressSql, new { UserId = userId })).ToList();

        var roles = user.Roles.Count > 0 ? user.Roles : new List<string> { user.Role };

        return new UserMeDto
        {
            id = user.UserId.ToString(),
            email = user.Email,
            displayName = user.DisplayName,
            roles = roles,
            phone = profile?.Phone,
            dateOfBirth = profile?.DateOfBirth?.ToString("yyyy-MM-dd"),
            gender = profile?.Gender,
            emergencyContactName = profile?.EmergencyContactName,
            emergencyContactPhone = profile?.EmergencyContactPhone,
            bio = profile?.Bio,
            profilePhotoUrl = profile?.ProfilePhotoUrl,
            buddyApplicationStatus = profile?.BuddyApplicationStatus ?? "None",
            buddyApplicationNotes = profile?.BuddyApplicationNotes,
            buddyLocalityId = profile?.BuddyLocalityId,
            addresses = addresses
        };
    }

    public async Task<bool> UpsertProfileAsync(int userId, UpdateUserMeRequest request)
    {
        var scope = request.updateScope?.Trim().ToLowerInvariant() ?? "all";
        var basicOnly = scope == "basic";
        var advancedOnly = scope == "advanced";

        if (!advancedOnly && string.IsNullOrWhiteSpace(request.displayName))
        {
            return false;
        }

        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var tx = connection.BeginTransaction();

        if (!string.IsNullOrWhiteSpace(request.displayName))
        {
            await _users.UpdateDisplayNameAsync(userId, request.displayName.Trim(), connection, tx);
        }

        DateOnly? dob = null;
        if (!string.IsNullOrWhiteSpace(request.dateOfBirth) && DateOnly.TryParse(request.dateOfBirth, out var parsed))
        {
            dob = parsed;
        }

        if (basicOnly || scope == "all")
        {
            const string mergeBasicSql = """
                MERGE dbo.UserProfiles AS target
                USING (SELECT @UserId AS UserId) AS source
                ON target.UserId = source.UserId
                WHEN MATCHED THEN
                    UPDATE SET
                        Phone = @Phone,
                        Bio = @Bio,
                        ProfilePhotoUrl = COALESCE(@ProfilePhotoUrl, target.ProfilePhotoUrl),
                        UpdatedAt = SYSUTCDATETIME()
                WHEN NOT MATCHED THEN
                    INSERT (UserId, Phone, Bio, ProfilePhotoUrl)
                    VALUES (@UserId, @Phone, @Bio, @ProfilePhotoUrl);
                """;

            await connection.ExecuteAsync(mergeBasicSql, new
            {
                UserId = userId,
                Phone = request.phone?.Trim(),
                Bio = request.bio?.Trim(),
                ProfilePhotoUrl = request.profilePhotoUrl?.Trim()
            }, tx);
        }

        if (advancedOnly || scope == "all")
        {
            const string mergeAdvancedSql = """
                MERGE dbo.UserProfiles AS target
                USING (SELECT @UserId AS UserId) AS source
                ON target.UserId = source.UserId
                WHEN MATCHED THEN
                    UPDATE SET
                        DateOfBirth = @DateOfBirth,
                        Gender = @Gender,
                        EmergencyContactName = @EmergencyContactName,
                        EmergencyContactPhone = @EmergencyContactPhone,
                        UpdatedAt = SYSUTCDATETIME()
                WHEN NOT MATCHED THEN
                    INSERT (UserId, DateOfBirth, Gender, EmergencyContactName, EmergencyContactPhone)
                    VALUES (@UserId, @DateOfBirth, @Gender, @EmergencyContactName, @EmergencyContactPhone);
                """;

            await connection.ExecuteAsync(mergeAdvancedSql, new
            {
                UserId = userId,
                DateOfBirth = dob?.ToDateTime(TimeOnly.MinValue),
                Gender = request.gender?.Trim(),
                EmergencyContactName = request.emergencyContactName?.Trim(),
                EmergencyContactPhone = request.emergencyContactPhone?.Trim()
            }, tx);

            const string deleteAddressesSql = "DELETE FROM dbo.UserAddresses WHERE UserId = @UserId";
            await connection.ExecuteAsync(deleteAddressesSql, new { UserId = userId }, tx);

            const string insertAddressSql = """
                INSERT INTO dbo.UserAddresses (UserId, Label, Line1, Line2, City, State, Pincode, IsDefault)
                VALUES (@UserId, @Label, @Line1, @Line2, @City, @State, @Pincode, @IsDefault)
                """;

            foreach (var address in request.addresses.Where(a => !string.IsNullOrWhiteSpace(a.line1)))
            {
                await connection.ExecuteAsync(insertAddressSql, new
                {
                    UserId = userId,
                    Label = string.IsNullOrWhiteSpace(address.label) ? "Home" : address.label.Trim(),
                    Line1 = address.line1.Trim(),
                    Line2 = address.line2?.Trim(),
                    City = address.city?.Trim() ?? "Pune",
                    State = address.state?.Trim() ?? "Maharashtra",
                    Pincode = address.pincode?.Trim() ?? "411000",
                    IsDefault = address.isDefault
                }, tx);
            }
        }

        tx.Commit();
        return true;
    }

    public async Task<string?> UpdateProfilePhotoAsync(int userId, string profilePhotoUrl)
    {
        const string sql = """
            MERGE dbo.UserProfiles AS target
            USING (SELECT @UserId AS UserId) AS source
            ON target.UserId = source.UserId
            WHEN MATCHED THEN
                UPDATE SET ProfilePhotoUrl = @ProfilePhotoUrl, UpdatedAt = SYSUTCDATETIME()
            WHEN NOT MATCHED THEN
                INSERT (UserId, ProfilePhotoUrl) VALUES (@UserId, @ProfilePhotoUrl);
            """;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId, ProfilePhotoUrl = profilePhotoUrl });
        return profilePhotoUrl;
    }

    public async Task<bool> ApplyBuddyAsync(int userId, BuddyApplyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.localityId) || string.IsNullOrWhiteSpace(request.bio))
        {
            return false;
        }

        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            MERGE dbo.UserProfiles AS target
            USING (SELECT @UserId AS UserId) AS source
            ON target.UserId = source.UserId
            WHEN MATCHED THEN
                UPDATE SET
                    Bio = @Bio,
                    BuddyApplicationStatus = N'Pending',
                    BuddyApplicationNotes = @Notes,
                    BuddyLocalityId = @LocalityId,
                    UpdatedAt = SYSUTCDATETIME()
            WHEN NOT MATCHED THEN
                INSERT (UserId, Bio, BuddyApplicationStatus, BuddyApplicationNotes, BuddyLocalityId)
                VALUES (@UserId, @Bio, N'Pending', @Notes, @LocalityId);
            """;

        var rows = await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            Bio = request.bio.Trim(),
            Notes = request.notes?.Trim(),
            LocalityId = request.localityId.Trim()
        });

        return rows > 0;
    }

    private sealed class ProfileRow
    {
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string BuddyApplicationStatus { get; set; } = "None";
        public string? BuddyApplicationNotes { get; set; }
        public string? BuddyLocalityId { get; set; }
    }
}
