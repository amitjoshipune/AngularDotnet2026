using AuthApi.Data;
using AuthApi.Models;
using Dapper;

namespace AuthApi.Repositories;

public sealed class UserDocumentRepository : IUserDocumentRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UserDocumentRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<UserDocumentDto>> GetForUserAsync(int userId)
    {
        const string sql = """
            SELECT
                DocumentId AS documentId,
                DocumentType AS documentType,
                FileName AS fileName,
                Status AS status,
                CONVERT(varchar(19), UploadedAt, 126) AS uploadedAt,
                CONVERT(varchar(19), VerifiedAt, 126) AS verifiedAt,
                RejectionReason AS rejectionReason
            FROM dbo.UserDocuments
            WHERE UserId = @UserId
            ORDER BY UploadedAt DESC
            """;

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<UserDocumentDto>(sql, new { UserId = userId });
        return rows.ToList();
    }

    public async Task<UserDocumentDto?> AddAsync(
        int userId,
        string documentType,
        string fileName,
        string storagePath,
        bool autoVerify)
    {
        const string sql = """
            INSERT INTO dbo.UserDocuments (UserId, DocumentType, FileName, StoragePath, Status, VerifiedAt)
            OUTPUT INSERTED.DocumentId
            VALUES (
                @UserId, @DocumentType, @FileName, @StoragePath,
                @Status,
                CASE WHEN @Status = N'Verified' THEN SYSUTCDATETIME() ELSE NULL END
            )
            """;

        var status = autoVerify ? DocumentStatus.Verified : DocumentStatus.Pending;

        using var connection = _connectionFactory.CreateConnection();
        var documentId = await connection.QuerySingleAsync<int>(sql, new
        {
            UserId = userId,
            DocumentType = documentType,
            FileName = fileName,
            StoragePath = storagePath,
            Status = status
        });

        var docs = await GetForUserAsync(userId);
        return docs.FirstOrDefault(d => d.documentId == documentId);
    }

    public async Task<bool> HasVerifiedDocumentsAsync(int userId, IEnumerable<string> documentTypes)
    {
        var types = documentTypes.ToList();
        if (types.Count == 0)
        {
            return true;
        }

        const string sql = """
            SELECT DocumentType
            FROM dbo.UserDocuments
            WHERE UserId = @UserId AND Status = N'Verified'
            """;

        using var connection = _connectionFactory.CreateConnection();
        var verified = (await connection.QueryAsync<string>(sql, new { UserId = userId }))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return types.All(verified.Contains);
    }

    public async Task<VerificationStatusDto> GetVerificationStatusAsync(int userId)
    {
        var documents = await GetForUserAsync(userId);
        var missing = DocumentTypes.RequiredForBuddyAccept
            .Where(type => !documents.Any(d =>
                string.Equals(d.documentType, type, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(d.status, DocumentStatus.Verified, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return new VerificationStatusDto
        {
            canAcceptBookings = true,
            missingDocuments = missing,
            documents = documents.ToList(),
            note = "ID documents are optional in this MVP build. Upload when ready."
        };
    }
}
