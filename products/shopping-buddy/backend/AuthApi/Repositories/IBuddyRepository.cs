using AuthApi.Models;

namespace AuthApi.Repositories;

public interface IBuddyRepository
{
    Task<IReadOnlyList<BuddyDto>> SearchAsync(BuddySearchQuery query);
    Task<BuddyDto?> GetByIdAsync(string buddyId);
}
