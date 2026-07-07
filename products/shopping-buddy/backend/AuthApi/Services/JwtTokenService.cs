using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public sealed class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponse CreateLoginResponse(AuthUser user)
    {
        var expiresMinutes = _configuration.GetValue<int>("Jwt:ExpiresMinutes", 60);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);

        var roleClaims = user.roles
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(role => new Claim(ClaimTypes.Role, role))
            .ToList();

        if (roleClaims.Count == 0 && !string.IsNullOrWhiteSpace(user.role))
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, user.role));
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.id),
            new(JwtRegisteredClaimNames.Email, user.email),
            new(JwtRegisteredClaimNames.Name, user.displayName),
            new("roles", string.Join(',', roleClaims.Select(c => c.Value)))
        };
        claims.AddRange(roleClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSigningKey()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new LoginResponse
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = $"refresh-{user.id}-{expiresAt.ToUnixTimeSeconds()}",
            expiresIn = expiresMinutes * 60,
            user = user
        };
    }

    public static string ResolvePrimaryRole(IEnumerable<string> roles)
    {
        var list = roles.ToList();
        if (list.Any(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase)))
        {
            return "Admin";
        }

        if (list.Any(r => string.Equals(r, "Buddy", StringComparison.OrdinalIgnoreCase)))
        {
            return "Buddy";
        }

        return list.FirstOrDefault() ?? "Customer";
    }

    public string GetSigningKey()
    {
        var key = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");
        }

        return key;
    }
}
