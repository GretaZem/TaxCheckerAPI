using Moq;
using TaxChecker.Application.Cities;
using TaxChecker.Application.Taxes;
using TaxChecker.Domain;

namespace TaxChecker.Tests;

public sealed class TaxServiceTests
{
    private readonly Mock<ICityRepository> _citiesMock = new();
    private readonly Mock<ITaxRuleRepository> _taxRulesMock = new();
    private readonly ITaxService _service;

    private const int KaunasCityId = 1;

    public TaxServiceTests()
    {
        _service = new TaxService(_citiesMock.Object, _taxRulesMock.Object);
    }

    private static List<TaxRule> CreateKaunasExampleRules()
    {
        return new List<TaxRule>
        {
            new(KaunasCityId, TaxRuleType.Yearly, 3.3m,
                new DateTime(2024,1,1), new DateTime(2024,12,31)),

            new(KaunasCityId, TaxRuleType.Monthly, 5m,
                new DateTime(2024,6,1), new DateTime(2024,6,30)),
            new(KaunasCityId, TaxRuleType.Monthly, 4m,
                new DateTime(2024,7,1), new DateTime(2024,7,31)),
            new(KaunasCityId, TaxRuleType.Monthly, 6m,
                new DateTime(2024,8,1), new DateTime(2024,8,31)),

            new(KaunasCityId, TaxRuleType.Weekly, 2.5m,
                new DateTime(2024,2,9), new DateTime(2024,2,15)),
            new(KaunasCityId, TaxRuleType.Weekly, 2.5m,
                new DateTime(2024,3,2), new DateTime(2024,3,8)),

            new(KaunasCityId, TaxRuleType.Daily, 1.5m,
                new DateTime(2024,6,1), new DateTime(2024,6,1)),

            new(KaunasCityId, TaxRuleType.Daily, 1.2m,
                new DateTime(2024,10,23), new DateTime(2024,10,23)),
        };
    }

    private void SetupRepoForRange(DateOnly from, DateOnly to)
    {
        var rules = CreateKaunasExampleRules();

        _taxRulesMock
            .Setup(r => r.GetRulesForCityInRangeAsync(
                KaunasCityId, from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rules
                .Where(x => x.CityId == KaunasCityId &&
                            x.ValidFrom.Date <= to.ToDateTime(TimeOnly.MinValue) &&
                            x.ValidTo.Date >= from.ToDateTime(TimeOnly.MinValue))
                .ToList());
    }

    [Theory]
    [InlineData("2024-01-01", 3.3)]
    [InlineData("2024-02-11", 2.5)]
    [InlineData("2024-06-01", 1.5)]
    [InlineData("2024-06-02", 5.0)]
    public async Task GetTaxRateAsync_UsesCorrectPriority(string dateString, decimal expectedRate)
    {
        var date = DateOnly.Parse(dateString);
        SetupRepoForRange(date, date);

        var result = await _service.GetTaxRateAsync(KaunasCityId, date);

        Assert.NotNull(result);
        Assert.Equal(expectedRate, result.Value);
    }

    [Fact]
    public async Task GetAverageTaxRateAsync_MatchesExample()
    {
        var from = new DateOnly(2024, 6, 1);
        var to = new DateOnly(2024, 9, 1);
        SetupRepoForRange(from, to);

        var avg = await _service.GetAverageTaxRateAsync(KaunasCityId, from, to);

        Assert.InRange(avg, 4.93m, 4.95m);
    }

    [Fact]
    public async Task GetTaxRateAsync_ReturnsNull_WhenNoRules()
    {
        _taxRulesMock.Setup(r => r.GetRulesForCityInRangeAsync(
                KaunasCityId,
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaxRule>());

        var result = await _service.GetTaxRateAsync(KaunasCityId, new DateOnly(2025, 1, 1));

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTaxRuleAsync_CallsRepository()
    {
        TaxRule? captured = null;

        _taxRulesMock.Setup(r => r.AddAsync(It.IsAny<TaxRule>(), It.IsAny<CancellationToken>()))
            .Callback<TaxRule, CancellationToken>((t, _) => captured = t)
            .Returns(Task.CompletedTask);

        await _service.CreateTaxRuleAsync(
            cityId: KaunasCityId,
            type: TaxRuleType.Yearly,
            rate: 3.3m,
            validFrom: new DateOnly(2024, 1, 1),
            validTo: new DateOnly(2024, 12, 31));

        _taxRulesMock.Verify(r => r.AddAsync(It.IsAny<TaxRule>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(captured);
        Assert.Equal(KaunasCityId, captured!.CityId);
    }

    [Fact]
    public async Task UpdateTaxRuleAsync_ReturnsFalse_WhenRuleNotFound()
    {
        _taxRulesMock.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaxRule?)null);

        var result = await _service.UpdateTaxRuleAsync(
            42, 5m,
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 12, 31));

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTaxRuleAsync_DeletesRule_WhenFound()
    {
        var rule = new TaxRule(1, TaxRuleType.Monthly, 5m,
            new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        _taxRulesMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rule);

        _taxRulesMock.Setup(r => r.DeleteAsync(rule, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var success = await _service.DeleteTaxRuleAsync(10);

        Assert.True(success);
        _taxRulesMock.Verify(r => r.DeleteAsync(rule, It.IsAny<CancellationToken>()), Times.Once);
    }
}
