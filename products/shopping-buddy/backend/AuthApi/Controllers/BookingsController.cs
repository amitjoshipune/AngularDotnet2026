using System.IdentityModel.Tokens.Jwt;
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
    private readonly IBookingRepository _bookings;
    private readonly IWebHostEnvironment _environment;

    public BookingsController(IBookingRepository bookings, IWebHostEnvironment environment)
    {
        _bookings = bookings;
        _environment = environment;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        var customerCheck = EnsureCustomerAccount();
        if (customerCheck is not null)
        {
            return customerCheck;
        }

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
            return Unauthorized(new { message = "Could not read your user id from the login token. Sign out and sign in again." });
        }

        try
        {
            var confirmation = await _bookings.CreateAsync(userId.Value, request);
            return confirmation is null
                ? BadRequest(new { message = "Invalid buddy or venue." })
                : Ok(confirmation);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                message = "Database error while saving the booking.",
                detail = _environment.IsDevelopment() ? ex.Message : null
            });
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var customerCheck = EnsureCustomerAccount();
        if (customerCheck is not null)
        {
            return customerCheck;
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Could not read your user id from the login token. Sign out and sign in again." });
        }

        try
        {
            var bookings = await _bookings.GetForCustomerAsync(userId.Value);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                message = "Database error while loading bookings.",
                detail = _environment.IsDevelopment() ? ex.Message : null
            });
        }
    }

    private IActionResult? EnsureCustomerAccount()
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
        if (string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return StatusCode(403, new
        {
            message = "Bookings are linked to Customer accounts only. Log in as customer@demo.com or senior@demo.com to book or view trips."
        });
    }

    private int? GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == JwtRegisteredClaimNames.Sub ||
            c.Type == "sub")?.Value;

        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
