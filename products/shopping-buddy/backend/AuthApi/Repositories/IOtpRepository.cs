namespace AuthApi.Repositories;

public interface IOtpRepository
{
    Task CreateAsync(string email, string code, string purpose, DateTime expiresAtUtc);
    Task<bool> TryConsumeAsync(string email, string code, string purpose);
}
