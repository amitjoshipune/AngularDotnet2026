using AuthApi.Models;

namespace AuthApi.Repositories;

public interface IUserRepository
{
    Task<UserRecord?> FindByEmailAsync(string email);
}
