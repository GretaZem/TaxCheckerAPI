using TaxChecker.Application.Cities;
using TaxChecker.Application.Taxes.Models;
using TaxChecker.Domain;

namespace TaxChecker.Application.Taxes;

public sealed class TaxService : ITaxService
{
    private readonly ICityRepository _cities;
    private readonly ITaxRuleRepository _taxRules;

    public TaxService(ICityRepository cities, ITaxRuleRepository taxRules)
    {
        _cities = cities;
        _taxRules = taxRules;
    }

    public async Task<decimal?> GetTaxRateAsync(
        int cityId,
        DateOnly date,
        CancellationToken ct = default)
    {
        // Assumption: if city doesn't exist, null is returned; controller can turn this into 404.
        var city = await _cities.GetByIdAsync(cityId, ct);
        if (city is null) return null;

        var rules = await _taxRules.GetRulesForCityOnDateAsync(cityId, date, ct);
        var best = SelectHighestPriorityRule(rules);

        return best?.Rate;
    }

    public async Task<IReadOnlyList<DailyTaxRateDto>> GetTaxScheduleAsync(
        int cityId,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default)
    {
        if (to < from)
            throw new ArgumentException("End date must be greater than or equal to start date.", nameof(to));

        var city = await _cities.GetByIdAsync(cityId, ct);
        if (city is null)
            return [];

        var rules = await _taxRules.GetRulesForCityInRangeAsync(cityId, from, to, ct);

        var result = new List<DailyTaxRateDto>();

        for (var current = from; current <= to; current = current.AddDays(1))
        {
            var applicable = rules
                .Where(r => IsRuleActiveOn(r, current))
                .ToList();

            var best = SelectHighestPriorityRule(applicable);

            // Assumption: if no rule applies for a day, Rate = null (no tax).
            result.Add(new DailyTaxRateDto(current, best?.Rate));
        }

        return result;
    }

    public async Task<decimal> GetAverageTaxRateAsync(
        int cityId,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default)
    {
        var schedule = await GetTaxScheduleAsync(cityId, from, to, ct);

        if (schedule.Count == 0)
            return 0m;

        // Assumption: days without tax (Rate == null) count as 0 for average.
        var sum = schedule.Sum(d => d.Rate ?? 0m);
        var average = sum / schedule.Count;

        return Math.Round(average, 2); // match example 4.94 format
    }

    public async Task<int> CreateTaxRuleAsync(
        int cityId,
        TaxRuleType type,
        decimal rate,
        DateOnly validFrom,
        DateOnly validTo,
        CancellationToken ct = default)
    {
        _ = await _cities.GetByIdAsync(cityId, ct)
            ?? throw new InvalidOperationException($"City with id {cityId} not found.");

        var rule = new TaxRule(
            cityId,
            type,
            rate,
            validFrom.ToDateTime(TimeOnly.MinValue),
            validTo.ToDateTime(TimeOnly.MinValue));

        await _taxRules.AddAsync(rule, ct);
        return rule.Id;
    }

    public async Task<bool> UpdateTaxRuleAsync(
        int id,
        decimal rate,
        DateOnly validFrom,
        DateOnly validTo,
        CancellationToken ct = default)
    {
        var existing = await _taxRules.GetByIdAsync(id, ct);
        if (existing is null) return false;

        existing.Update(
            rate,
            validFrom.ToDateTime(TimeOnly.MinValue),
            validTo.ToDateTime(TimeOnly.MinValue));

        await _taxRules.UpdateAsync(existing, ct);
        return true;
    }

    public async Task<bool> DeleteTaxRuleAsync(
        int id,
        CancellationToken ct = default)
    {
        var existing = await _taxRules.GetByIdAsync(id, ct);
        if (existing is null) return false;

        await _taxRules.DeleteAsync(existing, ct);
        return true;
    }

    private static TaxRule? SelectHighestPriorityRule(IReadOnlyCollection<TaxRule> rules)
    {
        if (rules.Count == 0) return null;

        return rules
            .OrderBy(r => (int)r.Type)
            .ThenByDescending(r => r.Rate) // tie-breaker: higher rate wins
            .FirstOrDefault();
    }

    private static bool IsRuleActiveOn(TaxRule rule, DateOnly date)
    {
        var d = date.ToDateTime(TimeOnly.MinValue);
        return rule.ValidFrom <= d && rule.ValidTo >= d;
    }
}
