using AuthApi.Repositories;

namespace AuthApi.Services;

public sealed class OtpService : IOtpService
{
    private readonly IOtpRepository _otpRepository;
    private readonly IOtpSender _otpSender;
    private readonly IConfiguration _configuration;

    public OtpService(IOtpRepository otpRepository, IOtpSender otpSender, IConfiguration configuration)
    {
        _otpRepository = otpRepository;
        _otpSender = otpSender;
        _configuration = configuration;
    }

    public async Task IssueAsync(string email, string purpose, string? extraMessage = null)
    {
        var code = GenerateCode();
        var expiresMinutes = _configuration.GetValue<int>("Otp:ExpiresMinutes", 10);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        await _otpRepository.CreateAsync(email, code, purpose, expiresAt);
        await _otpSender.SendAsync(email, purpose, code, extraMessage);
    }

    public Task<bool> ValidateAndConsumeAsync(string email, string code, string purpose) =>
        _otpRepository.TryConsumeAsync(email, code, purpose);

    private static string GenerateCode()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }
}
