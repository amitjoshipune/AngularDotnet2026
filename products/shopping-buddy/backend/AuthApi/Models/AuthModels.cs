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
    public string role { get; set; } = string.Empty;
}

public class AuthAccount
{
    public AuthAccount(string id, string email, string password, string displayName, string role)
    {
        this.id = id;
        this.email = email;
        this.password = password;
        this.displayName = displayName;
        this.role = role;
    }

    public string id { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string displayName { get; set; }
    public string role { get; set; }
}
