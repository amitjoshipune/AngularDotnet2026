namespace AuthApi.Services;

public interface IOtpSender
{
    Task SendAsync(string email, string purpose, string code, string? extraMessage = null);
}
