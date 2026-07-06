using AuthApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly CatalogRepository _catalog;

    public VenuesController(CatalogRepository catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? localityId)
    {
        try
        {
            var venues = await _catalog.GetVenuesAsync(localityId);
            return Ok(venues);
        }
        catch (Exception)
        {
            return StatusCode(503, new { message = "Database is unavailable. Run database/run-all.bat first." });
        }
    }
}
