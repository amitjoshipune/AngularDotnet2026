namespace AuthApi.Services;

public interface IDocumentStorageService
{
    Task<string> SaveAsync(int userId, string documentType, string originalFileName, Stream content);
}
