using Microsoft.AspNetCore.Mvc;
using TaxChecker.Application.Cities;

namespace TaxChecker.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _service;

    public CitiesController(ICityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCities(CancellationToken ct)
    {
        var cities = await _service.GetAllAsync(ct);
        return Ok(cities);
    }
}
