using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == JwtRegisteredClaimNames.Sub ||
            c.Type == "sub")?.Value;

        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public static bool HasRole(this ClaimsPrincipal user, string role) =>
        user.Claims.Any(c =>
            (c.Type == ClaimTypes.Role || c.Type == "role") &&
            string.Equals(c.Value, role, StringComparison.OrdinalIgnoreCase));
}
