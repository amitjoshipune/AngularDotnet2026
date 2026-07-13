using AuthApi.Models;
using AuthApi.Repositories;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;

    public AuthController(
        IUserRepository users,
        JwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        IOtpService otpService)
    {
        _users = users;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        UserRecord? account;
        try
        {
            account = await _users.FindByEmailAsync(request.email);
        }
        catch (Exception)
        {
            return DatabaseUnavailable();
        }

        if (account is null || !_passwordHasher.Verify(request.password, account.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!account.EmailVerified)
        {
            return StatusCode(403, new
            {
                message = "Email not verified. Check the API console for your OTP code, then verify at /verify-email.",
                requiresVerification = true,
                email = account.Email
            });
        }

        return Ok(_jwtTokenService.CreateLoginResponse(ToAuthUser(account)));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email) ||
            string.IsNullOrWhiteSpace(request.password) ||
            string.IsNullOrWhiteSpace(request.displayName))
        {
            return BadRequest(new { message = "Email, password, and display name are required." });
        }

        if (request.password.Length < 8)
        {
            return BadRequest(new { message = "Password must be at least 8 characters." });
        }

        try
        {
            if (await _users.EmailExistsAsync(request.email))
            {
                return Conflict(new { message = "An account with this email already exists." });
            }

            var passwordHash = _passwordHasher.Hash(request.password);
            await _users.CreateCustomerAsync(request.email, passwordHash, request.displayName);
            await _otpService.IssueAsync(request.email, OtpPurpose.EmailVerification);
        }
        catch (Exception)
        {
            return DatabaseUnavailable();
        }

        return Ok(new RegisterResponse
        {
            message = "Account created. Enter the OTP from the API console to verify your email.",
            email = request.email.Trim(),
            requiresVerification = true
        });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.code))
        {
            return BadRequest(new { message = "Email and verification code are required." });
        }

        UserRecord? account;
        try
        {
            account = await _users.FindByEmailAsync(request.email);
            if (account is null)
            {
                return BadRequest(new { message = "Invalid verification code." });
            }

            if (account.EmailVerified)
            {
                return Ok(_jwtTokenService.CreateLoginResponse(ToAuthUser(account)));
            }

            var valid = await _otpService.ValidateAndConsumeAsync(
                request.email,
                request.code,
                OtpPurpose.EmailVerification);

            if (!valid)
            {
                return BadRequest(new { message = "Invalid or expired verification code." });
            }

            await _users.SetEmailVerifiedAsync(account.UserId);
            account = await _users.FindByEmailAsync(request.email);
        }
        catch (Exception)
        {
            return DatabaseUnavailable();
        }

        if (account is null)
        {
            return BadRequest(new { message = "Invalid verification code." });
        }

        return Ok(_jwtTokenService.CreateLoginResponse(ToAuthUser(account)));
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.purpose))
        {
            return BadRequest(new { message = "Email and purpose are required." });
        }

        if (!IsSupportedPurpose(request.purpose))
        {
            return BadRequest(new { message = "Unsupported OTP purpose." });
        }

        try
        {
            var account = await _users.FindByEmailAsync(request.email);
            if (account is null)
            {
                return Ok(GenericOtpSentMessage());
            }

            if (string.Equals(request.purpose, OtpPurpose.EmailVerification, StringComparison.Ordinal) &&
                account.EmailVerified)
            {
                return BadRequest(new { message = "Email is already verified. You can sign in." });
            }

            await _otpService.IssueAsync(request.email, request.purpose);
        }
        catch (Exception)
        {
            return DatabaseUnavailable();
        }

        return Ok(GenericOtpSentMessage());
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email))
        {
            return BadRequest(new { message = "Email is required." });
        }

        try
        {
            var account = await _users.FindByEmailAsync(request.email);
            if (account is not null)
            {
                await _otpService.IssueAsync(request.email, OtpPurpose.PasswordReset);
            }
        }
        catch (Exception)
        {
            return DatabaseUnavailable();
        }

        return Ok(GenericOtpSentMessage());
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email) ||
            string.IsNullOrWhiteSpace(request.code) ||
            string.IsNullOrWhiteSpace(request.newPassword))
        {
            return BadRequest(new { message = "Email, code, and new password are required." });
        }

        if (request.newPassword.Length < 8)
        {
            return BadRequest(new { message = "Password must be at least 8 characters." });
        }

        try
        {
            var account = await _users.FindByEmailAsync(request.email);
            if (account is null)
            {
                return BadRequest(new { message = "Invalid or expired reset code." });
            }

            var valid = await _otpService.ValidateAndConsumeAsync(
                request.email,
                request.code,
                OtpPurpose.PasswordReset);

            if (!valid)
            {
                return BadRequest(new { message = "Invalid or expired reset code." });
            }

            var passwordHash = _passwordHasher.Hash(request.newPassword);
            await _users.UpdatePasswordAsync(account.UserId, passwordHash);
        }
        catch (Exception)
        {
            return DatabaseUnavailable();
        }

        return Ok(new MessageResponse
        {
            message = "Password updated. You can sign in with your new password."
        });
    }

    [HttpPost("forgot-login-id")]
    public async Task<IActionResult> ForgotLoginId([FromBody] ForgotLoginIdRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.displayName))
        {
            return BadRequest(new { message = "Display name is required." });
        }

        try
        {
            var account = await _users.FindByDisplayNameAsync(request.displayName);
            if (account is not null)
            {
                var message = $"Your login email is: {account.Email}";
                await _otpService.IssueAsync(account.Email, OtpPurpose.ForgotLoginId, message);
            }
        }
        catch (Exception)
        {
            return DatabaseUnavailable();
        }

        return Ok(new MessageResponse
        {
            message = "If an account matches that display name, your login email was sent to the registered address. Check the API console in dev."
        });
    }

    private static AuthUser ToAuthUser(UserRecord account)
    {
        var roles = account.Roles.Count > 0 ? account.Roles : new List<string> { account.Role };
        return new AuthUser
        {
            id = account.UserId.ToString(),
            email = account.Email,
            displayName = account.DisplayName,
            roles = roles,
            role = JwtTokenService.ResolvePrimaryRole(roles)
        };
    }

    private static bool IsSupportedPurpose(string purpose) =>
        string.Equals(purpose, OtpPurpose.EmailVerification, StringComparison.Ordinal) ||
        string.Equals(purpose, OtpPurpose.PasswordReset, StringComparison.Ordinal) ||
        string.Equals(purpose, OtpPurpose.ForgotLoginId, StringComparison.Ordinal);

    private static MessageResponse GenericOtpSentMessage() => new()
    {
        message = "If an account exists, a code was sent. In local dev, check the API console for [DEV OTP]."
    };

    private ObjectResult DatabaseUnavailable() => StatusCode(503, new
    {
        message = "Database is unavailable. Run database scripts including 009_account_auth.sql, then restart the API."
    });
}
