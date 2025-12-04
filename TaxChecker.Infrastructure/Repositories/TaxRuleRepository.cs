using Microsoft.EntityFrameworkCore;
using TaxChecker.Application.Taxes;
using TaxChecker.Domain;
using TaxChecker.Infrastructure.Data;

namespace TaxChecker.Infrastructure.Repositories;

internal sealed class TaxRuleRepository : ITaxRuleRepository
{
    private readonly AppDbContext _db;

    public TaxRuleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TaxRule>> GetRulesForCityOnDateAsync(
        int cityId,
        DateOnly date,
        CancellationToken ct = default)
    {
        var d = date.ToDateTime(TimeOnly.MinValue);

        return await _db.TaxRules
            .Where(r => r.CityId == cityId &&
                        r.ValidFrom <= d &&
                        r.ValidTo >= d)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TaxRule>> GetRulesForCityInRangeAsync(
        int cityId,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default)
    {
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt = to.ToDateTime(TimeOnly.MinValue);

        // Any rule that overlaps the range at all
        return await _db.TaxRules
            .Where(r => r.CityId == cityId &&
                        r.ValidFrom <= toDt &&
                        r.ValidTo >= fromDt)
            .ToListAsync(ct);
    }

    public Task<TaxRule?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.TaxRules.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task AddAsync(TaxRule rule, CancellationToken ct = default)
    {
        _db.TaxRules.Add(rule);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TaxRule rule, CancellationToken ct = default)
    {
        _db.TaxRules.Update(rule);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TaxRule rule, CancellationToken ct = default)
    {
        _db.TaxRules.Remove(rule);
        await _db.SaveChangesAsync(ct);
    }
}
