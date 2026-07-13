using AuthApi.Extensions;
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
    private static readonly RejectionReasonDto[] RejectionReasons =
    {
        new() { code = "ScheduleConflict", label = "Already booked at that time", requiresText = false },
        new() { code = "VenueTooFar", label = "Venue is too far for me", requiresText = false },
        new() { code = "PersonalEmergency", label = "Personal / family emergency", requiresText = false },
        new() { code = "SafetyConcern", label = "Safety concern about request", requiresText = false },
        new() { code = "Other", label = "Other reason", requiresText = true }
    };

    private readonly IBookingRepository _bookings;
    private readonly IUserDocumentRepository _documents;
    private readonly IWebHostEnvironment _environment;

    public BookingsController(
        IBookingRepository bookings,
        IUserDocumentRepository documents,
        IWebHostEnvironment environment)
    {
        _bookings = bookings;
        _documents = documents;
        _environment = environment;
    }

    [HttpGet("rejection-reasons")]
    [AllowAnonymous]
    public IActionResult GetRejectionReasons() => Ok(RejectionReasons);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        if (!User.HasRole("Customer"))
        {
            return CustomerRequired();
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

        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Could not read your user id from the login token. Sign out and sign in again." });
        }

        try
        {
            var (confirmation, isDuplicate) = await _bookings.CreateAsync(userId.Value, request);
            if (isDuplicate)
            {
                return Conflict(new
                {
                    message = "You already have a booking at this venue, date, and time slot."
                });
            }

            return confirmation is null
                ? BadRequest(new { message = "Invalid buddy or venue." })
                : Ok(confirmation);
        }
        catch (Exception ex)
        {
            return DatabaseError("saving the booking", ex);
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        if (!User.HasRole("Customer"))
        {
            return CustomerRequired();
        }

        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var bookings = await _bookings.GetForCustomerAsync(userId.Value);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return DatabaseError("loading bookings", ex);
        }
    }

    [HttpGet("buddy/incoming")]
    public async Task<IActionResult> GetBuddyIncoming()
    {
        if (!User.HasRole("Buddy"))
        {
            return StatusCode(403, new { message = "Buddy account required. Log in as meera@demo.com." });
        }

        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var buddyId = await _bookings.GetBuddyIdForUserAsync(userId.Value);
        if (buddyId is null)
        {
            return NotFound(new { message = "No buddy profile is linked to this login." });
        }

        try
        {
            var bookings = await _bookings.GetIncomingForBuddyUserAsync(userId.Value);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return DatabaseError("loading buddy requests", ex);
        }
    }

    [HttpPost("{bookingId}/confirm")]
    public async Task<IActionResult> Confirm(string bookingId)
    {
        if (!User.HasRole("Buddy"))
        {
            return StatusCode(403, new { message = "Buddy account required." });
        }

        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var hasDocs = await _documents.HasVerifiedDocumentsAsync(
                userId.Value,
                DocumentTypes.RequiredForBuddyAccept);

            if (!hasDocs)
            {
                return StatusCode(403, new
                {
                    message = "Verified Aadhaar and address proof are required before accepting bookings.",
                    missingDocuments = DocumentTypes.RequiredForBuddyAccept,
                    profileUrl = "/profile"
                });
            }

            var ok = await _bookings.ConfirmAsync(bookingId, userId.Value);
            return ok ? Ok(new { message = "Booking confirmed." }) : NotFound(new { message = "Pending booking not found." });
        }
        catch (Exception ex)
        {
            return DatabaseError("confirming booking", ex);
        }
    }

    [HttpPost("{bookingId}/reject")]
    public async Task<IActionResult> Reject(string bookingId, [FromBody] RejectBookingRequest request)
    {
        if (!User.HasRole("Buddy"))
        {
            return StatusCode(403, new { message = "Buddy account required." });
        }

        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.reasonCode))
        {
            return BadRequest(new { message = "Rejection reason code is required." });
        }

        try
        {
            var ok = await _bookings.RejectAsync(bookingId, userId.Value, request);
            if (!ok)
            {
                return BadRequest(new
                {
                    message = "Could not reject booking. Check reason code (Other needs free text) and that status is PendingBuddy."
                });
            }

            return Ok(new { message = "Booking rejected." });
        }
        catch (Exception ex)
        {
            return DatabaseError("rejecting booking", ex);
        }
    }

    [HttpPost("{bookingId}/cancel")]
    public async Task<IActionResult> Cancel(string bookingId)
    {
        if (!User.HasRole("Customer"))
        {
            return CustomerRequired();
        }

        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var ok = await _bookings.CancelAsync(bookingId, userId.Value);
            if (!ok)
            {
                return BadRequest(new
                {
                    message = "Could not cancel booking. Only PendingBuddy or Confirmed trips can be cancelled."
                });
            }

            return Ok(new { message = "Booking cancelled." });
        }
        catch (Exception ex)
        {
            return DatabaseError("cancelling booking", ex);
        }
    }

    private IActionResult CustomerRequired() =>
        StatusCode(403, new
        {
            message = "Customer role required. Use customer@demo.com or a buddy account that also has Customer role."
        });

    private IActionResult DatabaseError(string action, Exception ex) =>
        StatusCode(503, new
        {
            message = $"Database error while {action}.",
            detail = _environment.IsDevelopment() ? ex.Message : null
        });
}
