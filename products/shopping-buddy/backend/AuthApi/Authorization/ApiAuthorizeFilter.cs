using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthApi.Authorization;

public sealed class ApiAuthorizeFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Authorization token is required." });
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (!IsValidToken(token))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid or expired token." });
        }
    }

    private static bool IsValidToken(string token)
    {
        return token.StartsWith("demo-token-", StringComparison.OrdinalIgnoreCase) || token.StartsWith("eyJ", StringComparison.OrdinalIgnoreCase);
    }
}
