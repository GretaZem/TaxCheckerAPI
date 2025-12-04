using System.ComponentModel.DataAnnotations;
using TaxChecker.Domain;

namespace TaxChecker.API.Models;

public sealed class CreateTaxRuleRequest
{
    [Required]
    public TaxRuleType Type { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Rate { get; set; }

    [Required]
    public DateOnly ValidFrom { get; set; }

    [Required]
    public DateOnly ValidTo { get; set; }
}