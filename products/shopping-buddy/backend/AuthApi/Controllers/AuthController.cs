using Microsoft.AspNetCore.Mvc;
using AuthApi.Models;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly List<AuthAccount> HardcodedUsers = new()
    {
        new AuthAccount("1", "admin@example.com", "Admin@123", "Admin User", "Admin"),
        new AuthAccount("2", "tester@example.com", "Tester@123", "Tester User", "Tester"),
        new AuthAccount("3", "manager@example.com", "Manager@123", "Manager User", "Manager")
    };

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var account = HardcodedUsers.FirstOrDefault(x =>
            string.Equals(x.email, request.email.Trim(), StringComparison.OrdinalIgnoreCase) &&
            x.password == request.password);

        if (account is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var expiresIn = 3600;
        var exp = DateTimeOffset.UtcNow.AddSeconds(expiresIn).ToUnixTimeSeconds();
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var jwtToken = CreateMockJwt(
            sub: account.id,
            email: account.email,
            name: account.displayName,
            role: account.role,
            exp: exp,
            iat: iat
        );

        var response = new LoginResponse
        {
            accessToken = jwtToken,
            refreshToken = $"refresh-{account.id}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            expiresIn = expiresIn,
            user = new AuthUser
            {
                id = account.id,
                email = account.email,
                displayName = account.displayName,
                role = account.role
            }
        };

        return Ok(response);
    }

    private static string CreateMockJwt(string sub, string email, string name, string role, long exp, long iat)
    {
        var header = Base64UrlEncode(System.Text.Json.JsonSerializer.Serialize(new { alg = "HS256", typ = "JWT" }));
        var payload = Base64UrlEncode(System.Text.Json.JsonSerializer.Serialize(new { sub, email, name, role, exp, iat }));
        var signature = "mock-signature";
        return $"{header}.{payload}.{signature}";
    }

    private static string Base64UrlEncode(string input)
    {
        var data = System.Text.Encoding.UTF8.GetBytes(input);
        var base64 = Convert.ToBase64String(data);
        return base64.Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}
