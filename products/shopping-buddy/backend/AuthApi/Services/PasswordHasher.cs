namespace AuthApi.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string storedPassword)
    {
        if (storedPassword.StartsWith("$2", StringComparison.Ordinal))
        {
            return BCrypt.Net.BCrypt.Verify(password, storedPassword);
        }

        return string.Equals(password, storedPassword, StringComparison.Ordinal);
    }
}
