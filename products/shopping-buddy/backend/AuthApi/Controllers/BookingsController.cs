using System.Security.Claims;
using AuthApi.Models;
using AuthApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly BookingRepository _bookings;

    public BookingsController(BookingRepository bookings)
    {
        _bookings = bookings;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        if (!request.acceptedSafetyRules)
        {
            return BadRequest(new { message = "You must accept the safety rules before booking." });
        }

        if (string.IsNullOrWhiteSpace(request.buddyId) ||
            string.IsNullOrWhiteSpace(request.venueId) ||
            string.IsNullOrWhiteSpace(request.date) ||
            string.IsNullOrWhiteSpace(request.timeSlot) ||
            string.IsNullOrWhiteSpace(request.activityType))
        {
            return BadRequest(new { message = "Buddy, venue, date, time slot, and activity type are required." });
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var confirmation = await _bookings.CreateAsync(userId.Value, request);
            return confirmation is null
                ? BadRequest(new { message = "Invalid buddy or venue." })
                : Ok(confirmation);
        }
        catch (Exception)
        {
            return StatusCode(503, new { message = "Database is unavailable. Run database/run-all.bat first." });
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var bookings = await _bookings.GetForCustomerAsync(userId.Value);
            return Ok(bookings);
        }
        catch (Exception)
        {
            return StatusCode(503, new { message = "Database is unavailable. Run database/run-all.bat first." });
        }
    }

    private int? GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return int.TryParse(sub, out var userId) ? userId : null;
    }
}
