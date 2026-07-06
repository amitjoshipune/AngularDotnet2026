using AuthApi.Models;
using AuthApi.Repositories;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserRepository _users;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(UserRepository users, JwtTokenService jwtTokenService)
    {
        _users = users;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        UserRecord? account;
        try
        {
            account = await _users.FindByEmailAsync(request.email);
        }
        catch (Exception)
        {
            return StatusCode(503, new
            {
                message = "Database is unavailable. Run database/run-all.bat against LocalDB, then restart the API."
            });
        }

        if (account is null || account.Password != request.password)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var user = new AuthUser
        {
            id = account.UserId.ToString(),
            email = account.Email,
            displayName = account.DisplayName,
            role = account.Role
        };

        return Ok(_jwtTokenService.CreateLoginResponse(user));
    }
}
