namespace TaxChecker.Application.Taxes.Models;

public sealed record DailyTaxRateDto(
    DateOnly Date,
    decimal? Rate);
