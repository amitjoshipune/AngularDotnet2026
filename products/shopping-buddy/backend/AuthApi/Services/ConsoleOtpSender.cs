namespace AuthApi.Services;

public sealed class ConsoleOtpSender : IOtpSender
{
    private readonly ILogger<ConsoleOtpSender> _logger;

    public ConsoleOtpSender(ILogger<ConsoleOtpSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string email, string purpose, string code, string? extraMessage = null)
    {
        if (!string.IsNullOrWhiteSpace(extraMessage))
        {
            _logger.LogInformation(
                "[DEV OTP] {Purpose} for {Email}: {Code} — {ExtraMessage}",
                purpose,
                email,
                code,
                extraMessage);
        }
        else
        {
            _logger.LogInformation(
                "[DEV OTP] {Purpose} for {Email}: {Code}",
                purpose,
                email,
                code);
        }

        return Task.CompletedTask;
    }
}
