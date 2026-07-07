using AuthApi.Models;

namespace AuthApi.Repositories;

public interface IBookingRepository
{
    Task<(BookingConfirmationDto? confirmation, bool isDuplicate)> CreateAsync(int customerUserId, CreateBookingRequest request);
    Task<IReadOnlyList<BookingListItemDto>> GetForCustomerAsync(int customerUserId);
    Task<IReadOnlyList<BuddyIncomingBookingDto>> GetIncomingForBuddyUserAsync(int buddyUserId);
    Task<string?> GetBuddyIdForUserAsync(int userId);
    Task<bool> ConfirmAsync(string bookingId, int buddyUserId);
    Task<bool> RejectAsync(string bookingId, int buddyUserId, RejectBookingRequest request);
}
