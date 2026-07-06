using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> Store = new()
    {
        new ItemDto { Id = 1, Name = "Alpha Sprint", Category = "Planning", Status = "Active", CreatedAt = DateTime.UtcNow.AddDays(-5) },
        new ItemDto { Id = 2, Name = "Beta Release", Category = "Release", Status = "Review", CreatedAt = DateTime.UtcNow.AddDays(-2) },
        new ItemDto { Id = 3, Name = "Gamma QA", Category = "Testing", Status = "Done", CreatedAt = DateTime.UtcNow.AddDays(-1) }
    };

    [HttpGet]
    [Authorize]
    public IActionResult Get([FromQuery] string? search)
    {
        var items = string.IsNullOrWhiteSpace(search)
            ? Store
            : Store.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || x.Category.Contains(search, StringComparison.OrdinalIgnoreCase));

        return Ok(items.OrderByDescending(x => x.CreatedAt).ToList());
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        var item = Store.FirstOrDefault(x => x.Id == id);
        return item is null ? NotFound(new { message = "Item not found." }) : Ok(item);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Create([FromBody] ItemCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Category))
        {
            return BadRequest(new { message = "Name and category are required." });
        }

        var item = new ItemDto
        {
            Id = Store.Max(x => x.Id) + 1,
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Active" : request.Status.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        Store.Add(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public IActionResult Update(int id, [FromBody] ItemUpdateRequest request)
    {
        var existing = Store.FirstOrDefault(x => x.Id == id);
        if (existing is null)
        {
            return NotFound(new { message = "Item not found." });
        }

        existing.Name = request.Name.Trim();
        existing.Category = request.Category.Trim();
        existing.Status = request.Status.Trim();
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var existing = Store.FirstOrDefault(x => x.Id == id);
        if (existing is null)
        {
            return NotFound(new { message = "Item not found." });
        }

        Store.Remove(existing);
        return Ok(new { message = "Item deleted." });
    }
}
