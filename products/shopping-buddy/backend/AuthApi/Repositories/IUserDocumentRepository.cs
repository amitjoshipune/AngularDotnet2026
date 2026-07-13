using AuthApi.Models;

namespace AuthApi.Repositories;

public interface IUserDocumentRepository
{
    Task<IReadOnlyList<UserDocumentDto>> GetForUserAsync(int userId);
    Task<UserDocumentDto?> AddAsync(int userId, string documentType, string fileName, string storagePath, bool autoVerify);
    Task<bool> HasVerifiedDocumentsAsync(int userId, IEnumerable<string> documentTypes);
    Task<VerificationStatusDto> GetVerificationStatusAsync(int userId);
}
