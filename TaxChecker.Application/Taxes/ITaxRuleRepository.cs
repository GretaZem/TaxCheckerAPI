using TaxChecker.Domain;

namespace TaxChecker.Application.Taxes;

public interface ITaxRuleRepository
{
    Task<IReadOnlyList<TaxRule>> GetRulesForCityOnDateAsync(
        int cityId,
        DateOnly date,
        CancellationToken ct = default);

    Task<IReadOnlyList<TaxRule>> GetRulesForCityInRangeAsync(
        int cityId,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default);

    Task<TaxRule?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(TaxRule rule, CancellationToken ct = default);
    Task UpdateAsync(TaxRule rule, CancellationToken ct = default);
    Task DeleteAsync(TaxRule rule, CancellationToken ct = default);
}