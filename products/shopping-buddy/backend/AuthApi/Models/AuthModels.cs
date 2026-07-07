namespace AuthApi.Models;

public class LoginRequest
{
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string accessToken { get; set; } = string.Empty;
    public string refreshToken { get; set; } = string.Empty;
    public int expiresIn { get; set; }
    public AuthUser user { get; set; } = new();
}

public class AuthUser
{
    public string id { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string displayName { get; set; } = string.Empty;
    /** Primary role for legacy UI; prefer roles[] for checks. */
    public string role { get; set; } = string.Empty;
    public List<string> roles { get; set; } = new();
}

public class UserRecord
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
