using AuthApi.Models;

namespace AuthApi.Repositories;

public interface IUserProfileRepository
{
    Task<UserMeDto?> GetMeAsync(int userId);
    Task<bool> UpsertProfileAsync(int userId, UpdateUserMeRequest request);
    Task<string?> UpdateProfilePhotoAsync(int userId, string profilePhotoUrl);
    Task<bool> ApplyBuddyAsync(int userId, BuddyApplyRequest request);
}
