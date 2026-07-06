using AuthApi.Models;
using AuthApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuddiesController : ControllerBase
{
    private readonly BuddyRepository _buddies;

    public BuddiesController(BuddyRepository buddies)
    {
        _buddies = buddies;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] BuddySearchQuery query)
    {
        try
        {
            var results = await _buddies.SearchAsync(query);
            return Ok(results);
        }
        catch (Exception)
        {
            return StatusCode(503, new { message = "Database is unavailable. Run database/run-all.bat first." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var buddy = await _buddies.GetByIdAsync(id);
            return buddy is null ? NotFound(new { message = "Buddy not found." }) : Ok(buddy);
        }
        catch (Exception)
        {
            return StatusCode(503, new { message = "Database is unavailable. Run database/run-all.bat first." });
        }
    }
}
