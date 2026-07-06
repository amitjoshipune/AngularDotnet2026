using AuthApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocalitiesController : ControllerBase
{
    private readonly ICatalogRepository _catalog;

    public LocalitiesController(ICatalogRepository catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var localities = await _catalog.GetLocalitiesAsync();
            return Ok(localities);
        }
        catch (Exception)
        {
            return StatusCode(503, new { message = "Database is unavailable. Run database/run-all.bat first." });
        }
    }
}
