using CalcConverter.Core;

namespace CalcConverter.Tests;

/// <summary>
/// Unit tests for <see cref="CurrencyConverter"/>.
/// </summary>
public class CurrencyConverterTests
{
    // ---------- Test data ----------

    // (amount in GHS, cedis per dollar, expected USD rounded to 2 places)
    // Money is shown to the cent, so we compare at 2 decimal places.
    public static TheoryData<decimal, decimal, decimal> ConversionCases => new()
    {
        { 155m, 15.5m, 10m },     // divides evenly
        { 100m, 12.5m, 8m },
        { 0m, 15.5m, 0m },        // nothing to convert
        { 100m, 15.5m, 6.45m },   // 6.4516... rounded to the cent
        { 1m, 15.5m, 0.06m },     // small amounts still work
    };

    // Rates that make no sense: zero would divide by zero,
    // negatives have no real-world meaning.
    public static TheoryData<decimal> InvalidRates => new()
    {
        0m,
        -1m,
        -15.5m,
    };


    // ---------- Conversions ----------

    [Theory]
    [MemberData(nameof(ConversionCases))]
    public void GhsToUsd_ReturnsExpectedAmount(decimal amountGhs, decimal rate, decimal expectedUsd)
    {
        var usd = CurrencyConverter.GhsToUsd(amountGhs, rate);

        Assert.Equal(expectedUsd, Math.Round(usd, 2));
    }


    // ---------- Invalid input ----------

    [Theory]
    [MemberData(nameof(InvalidRates))]
    public void GhsToUsd_ZeroOrNegativeRate_ThrowsWithClearMessage(decimal rate)
    {
        var ex = Assert.Throws<ArgumentException>(() => CurrencyConverter.GhsToUsd(100m, rate));

        Assert.Equal("Exchange rate must be greater than zero.", ex.Message);
    }

    [Fact]
    public void GhsToUsd_NegativeAmount_ThrowsWithClearMessage()
    {
        var ex = Assert.Throws<ArgumentException>(() => CurrencyConverter.GhsToUsd(-50m, 15.5m));

        Assert.Equal("Amount cannot be negative.", ex.Message);
    }
}