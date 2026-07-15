using AuthApi.Extensions;
using AuthApi.Models;
using AuthApi.Repositories;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private static readonly HashSet<string> AllowedDocumentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        DocumentTypes.Aadhaar,
        DocumentTypes.AddressProof,
        DocumentTypes.Pan
    };

    private readonly IUserProfileRepository _profiles;
    private readonly IUserDocumentRepository _documents;
    private readonly IDocumentStorageService _storage;
    private readonly IWebHostEnvironment _environment;

    public UsersController(
        IUserProfileRepository profiles,
        IUserDocumentRepository documents,
        IDocumentStorageService storage,
        IWebHostEnvironment environment)
    {
        _profiles = profiles;
        _documents = documents;
        _storage = storage;
        _environment = environment;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var profile = await _profiles.GetMeAsync(userId.Value);
            return profile is null ? NotFound() : Ok(profile);
        }
        catch (Exception ex)
        {
            return DatabaseError("loading profile", ex);
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserMeRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var ok = await _profiles.UpsertProfileAsync(userId.Value, request);
            if (!ok)
            {
                return BadRequest(new { message = "Display name is required." });
            }

            var profile = await _profiles.GetMeAsync(userId.Value);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            return DatabaseError("saving profile", ex);
        }
    }

    [HttpPost("me/buddy-apply")]
    public async Task<IActionResult> ApplyBuddy([FromBody] BuddyApplyRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (!User.HasRole("Customer"))
        {
            return StatusCode(403, new { message = "Customer account required to apply as a shopping buddy." });
        }

        try
        {
            var ok = await _profiles.ApplyBuddyAsync(userId.Value, request);
            if (!ok)
            {
                return BadRequest(new { message = "Locality and bio are required." });
            }

            return Ok(new
            {
                message = "Application submitted. We will review and contact you. ID upload is optional for now."
            });
        }
        catch (Exception ex)
        {
            return DatabaseError("submitting buddy application", ex);
        }
    }

    [HttpGet("me/verification-status")]
    public async Task<IActionResult> GetVerificationStatus()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var status = await _documents.GetVerificationStatusAsync(userId.Value);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return DatabaseError("loading verification status", ex);
        }
    }

    [HttpGet("me/documents")]
    public async Task<IActionResult> GetDocuments()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var docs = await _documents.GetForUserAsync(userId.Value);
            return Ok(docs);
        }
        catch (Exception ex)
        {
            return DatabaseError("loading documents", ex);
        }
    }

    [HttpPost("me/documents")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocument([FromForm] string documentType, IFormFile? file)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(documentType) || !AllowedDocumentTypes.Contains(documentType))
        {
            return BadRequest(new { message = "documentType must be Aadhaar, AddressProof, or PAN." });
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "File is required." });
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var storagePath = await _storage.SaveAsync(userId.Value, documentType, file.FileName, stream);
            var autoVerify = _environment.IsDevelopment();
            var created = await _documents.AddAsync(userId.Value, documentType, file.FileName, storagePath, autoVerify);

            return Ok(new
            {
                message = autoVerify
                    ? "Document uploaded and auto-verified in Development."
                    : "Document uploaded and pending verification.",
                document = created
            });
        }
        catch (Exception ex)
        {
            return DatabaseError("uploading document", ex);
        }
    }

    [HttpPost("me/profile-photo")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile? file)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Photo file is required." });
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var storagePath = await _storage.SaveAsync(userId.Value, "ProfilePhoto", file.FileName, stream);
            var publicUrl = "/" + storagePath.TrimStart('/');
            await _profiles.UpdateProfilePhotoAsync(userId.Value, publicUrl);
            var profile = await _profiles.GetMeAsync(userId.Value);
            return Ok(new { message = "Profile photo updated.", profilePhotoUrl = publicUrl, profile });
        }
        catch (Exception ex)
        {
            return DatabaseError("uploading profile photo", ex);
        }
    }

    private ObjectResult DatabaseError(string action, Exception ex) =>
        StatusCode(503, new
        {
            message = $"Database error while {action}. Run migrations 010–012 in SSMS if tables or columns are missing.",
            detail = _environment.IsDevelopment() ? ex.Message : null
        });
}
