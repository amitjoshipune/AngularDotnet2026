namespace AuthApi.Services;

public interface IOtpService
{
    Task IssueAsync(string email, string purpose, string? extraMessage = null);
    Task<bool> ValidateAndConsumeAsync(string email, string code, string purpose);
}
