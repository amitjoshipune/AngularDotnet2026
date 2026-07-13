namespace AuthApi.Services;

public sealed class LocalDocumentStorageService : IDocumentStorageService
{
    private readonly string _uploadRoot;

    public LocalDocumentStorageService(IWebHostEnvironment environment)
    {
        _uploadRoot = Path.Combine(environment.ContentRootPath, "uploads", "documents");
        Directory.CreateDirectory(_uploadRoot);
    }

    public async Task<string> SaveAsync(int userId, string documentType, string originalFileName, Stream content)
    {
        var safeName = Path.GetFileName(originalFileName);
        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "upload.bin";
        }

        var userFolder = Path.Combine(_uploadRoot, userId.ToString());
        Directory.CreateDirectory(userFolder);

        var storedName = $"{documentType}-{DateTime.UtcNow:yyyyMMddHHmmss}-{safeName}";
        var fullPath = Path.Combine(userFolder, storedName);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream);

        return Path.Combine("uploads", "documents", userId.ToString(), storedName).Replace('\\', '/');
    }
}
