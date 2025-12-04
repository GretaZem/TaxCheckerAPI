using System.ComponentModel.DataAnnotations;

namespace TaxChecker.API.Models;

public sealed class UpdateTaxRuleRequest
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Rate { get; set; }

    [Required]
    public DateOnly ValidFrom { get; set; }

    [Required]
    public DateOnly ValidTo { get; set; }
}