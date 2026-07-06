using AuthApi.Models;

namespace AuthApi.Repositories;

public interface ICatalogRepository
{
    Task<IReadOnlyList<LocalityDto>> GetLocalitiesAsync();
    Task<IReadOnlyList<VenueDto>> GetVenuesAsync(string? localityId);
    Task<string?> GetLocalityNameAsync(string localityId);
}
