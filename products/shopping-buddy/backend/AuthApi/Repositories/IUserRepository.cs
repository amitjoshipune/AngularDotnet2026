using AuthApi.Models;

namespace AuthApi.Repositories;

public interface IUserRepository
{
    Task<UserRecord?> FindByEmailAsync(string email);
    Task<UserRecord?> FindByDisplayNameAsync(string displayName);
    Task<UserRecord?> FindByIdAsync(int userId);
    Task<bool> EmailExistsAsync(string email);
    Task<int> CreateCustomerAsync(string email, string passwordHash, string displayName);
    Task SetEmailVerifiedAsync(int userId);
    Task UpdatePasswordAsync(int userId, string passwordHash);
    Task UpdateDisplayNameAsync(int userId, string displayName, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction);
}
