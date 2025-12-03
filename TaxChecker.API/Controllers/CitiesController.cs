using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxChecker.Infrastructure.Data;

namespace TaxChecker.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CitiesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _db.Cities
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(cities);
    }
}
