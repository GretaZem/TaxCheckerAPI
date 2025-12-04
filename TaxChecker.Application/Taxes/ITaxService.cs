using TaxChecker.Application.Taxes.Models;
using TaxChecker.Domain;

namespace TaxChecker.Application.Taxes;

public interface ITaxService
{
    // Public / user-facing
    Task<decimal?> GetTaxRateAsync(
        int cityId,
        DateOnly date,
        CancellationToken ct = default);

    Task<IReadOnlyList<DailyTaxRateDto>> GetTaxScheduleAsync(
        int cityId,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default);

    Task<decimal> GetAverageTaxRateAsync(
        int cityId,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default);

    // Admin operations for controllers using "Admin" header
    Task<int> CreateTaxRuleAsync(
        int cityId,
        TaxRuleType type,
        decimal rate,
        DateOnly validFrom,
        DateOnly validTo,
        CancellationToken ct = default);

    Task<bool> UpdateTaxRuleAsync(
        int id,
        decimal rate,
        DateOnly validFrom,
        DateOnly validTo,
        CancellationToken ct = default);

    Task<bool> DeleteTaxRuleAsync(
        int id,
        CancellationToken ct = default);
}
