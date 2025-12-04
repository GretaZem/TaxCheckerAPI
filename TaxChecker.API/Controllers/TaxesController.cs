using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxChecker.API.Models;
using TaxChecker.Application.Taxes;

namespace TaxChecker.API.Controllers;

[ApiController]
[Route("api/cities/{cityId:int}/taxes")]
public sealed class TaxesController : ControllerBase
{
    private readonly ITaxService _taxService;

    public TaxesController(ITaxService taxService)
    {
        _taxService = taxService;
    }

    /// <summary>
    /// Get the tax rate applied for a specific city on a given date.
    /// </summary>
    [HttpGet("rate")]
    public async Task<IActionResult> GetRate(
        int cityId,
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var result = await _taxService.GetTaxRateAsync(cityId, date, ct);

        if (result is null)
            return NotFound($"No tax rule applies for city {cityId} on {date:yyyy-MM-dd}.");

        return Ok(result);
    }

    /// <summary>
    /// Get the daily tax schedule for a city over a date range.
    /// </summary>
    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule(
        int cityId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken ct)
    {
        if (to < from)
            return BadRequest("The 'to' date must be greater than or equal to the 'from' date.");

        var schedule = await _taxService.GetTaxScheduleAsync(cityId, from, to, ct);

        if (schedule.Count == 0)
            return NotFound($"No tax rules found for city {cityId} in the given date range.");

        return Ok(schedule);
    }

    /// <summary>
    /// Calculate the average tax rate for a city over a date range.
    /// </summary>
    [HttpGet("average")]
    public async Task<IActionResult> GetAverage(
        int cityId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken ct)
    {
        if (to < from)
            return BadRequest("The 'to' date must be greater than or equal to the 'from' date.");

        var average = await _taxService.GetAverageTaxRateAsync(cityId, from, to, ct);

        return Ok(average);
    }

    /// <summary>
    /// Create a new tax rule for the specified city. ADMIN ONLY.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(
        int cityId,
        [FromBody] CreateTaxRuleRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var id = await _taxService.CreateTaxRuleAsync(
                cityId,
                request.Type,
                request.Rate,
                request.ValidFrom,
                request.ValidTo,
                ct);

            return CreatedAtAction(
                nameof(GetRate),
                new { cityId, date = request.ValidFrom },
                new { id });
        }
        catch (InvalidOperationException ex)
        {
            // e.g. City not found
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            // domain validation (ValidTo < ValidFrom, rate <= 0, etc.)
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing tax rule. ADMIN ONLY.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(
        int cityId,
        int id,
        [FromBody] UpdateTaxRuleRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var success = await _taxService.UpdateTaxRuleAsync(
                id,
                request.Rate,
                request.ValidFrom,
                request.ValidTo,
                ct);

            if (!success)
                return NotFound($"Tax rule with id {id} was not found.");

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete an existing tax rule. ADMIN ONLY.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(
        int cityId,
        int id,
        CancellationToken ct)
    {
        var success = await _taxService.DeleteTaxRuleAsync(id, ct);

        if (!success)
            return NotFound($"Tax rule with id {id} was not found.");

        return NoContent();
    }
}
