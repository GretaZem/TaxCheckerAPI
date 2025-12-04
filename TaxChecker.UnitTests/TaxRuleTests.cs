using TaxChecker.Domain;

namespace TaxChecker.Tests;

public sealed class TaxRuleTests
{
    [Fact]
    public void Ctor_Throws_WhenValidToBeforeValidFrom()
    {
        var from = new DateTime(2024, 1, 10);
        var to = new DateTime(2024, 1, 1);

        var ex = Assert.Throws<ArgumentException>(() =>
            new TaxRule(cityId: 1,
                type: TaxRuleType.Yearly,
                rate: 3.3m,
                validFrom: from,
                validTo: to));

        Assert.Contains("Start date must be <= end date.", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Ctor_Throws_WhenRateNonPositive()
    {
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 31);

        var ex = Assert.Throws<ArgumentException>(() =>
            new TaxRule(1, TaxRuleType.Yearly, 0m, from, to));

        Assert.Contains("Rate", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Update_ChangesCoreFields()
    {
        var rule = new TaxRule(1, TaxRuleType.Monthly, 5m,
            new DateTime(2024, 6, 1),
            new DateTime(2024, 6, 30));

        rule.Update(
            rate: 6m,
            validFrom: new DateTime(2024, 7, 1),
            validTo: new DateTime(2024, 7, 31));

        Assert.Equal(6m, rule.Rate);
        Assert.Equal(new DateTime(2024, 7, 1), rule.ValidFrom);
        Assert.Equal(new DateTime(2024, 7, 31), rule.ValidTo);
    }
}
