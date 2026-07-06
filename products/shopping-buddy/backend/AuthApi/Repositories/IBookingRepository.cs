using AuthApi.Models;

namespace AuthApi.Repositories;

public interface IBookingRepository
{
    Task<BookingConfirmationDto?> CreateAsync(int customerUserId, CreateBookingRequest request);
    Task<IReadOnlyList<BookingListItemDto>> GetForCustomerAsync(int customerUserId);
}
