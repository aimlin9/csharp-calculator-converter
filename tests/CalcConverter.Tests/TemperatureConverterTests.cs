using CalcConverter.Core;

namespace CalcConverter.Tests;

/// <summary>
/// Unit tests for <see cref="TemperatureConverter"/>.
/// </summary>
public class TemperatureConverterTests
{
    // ---------- Test data ----------
    // Each pair is a well-known reference point, so a reader can check
    // the expected values without a calculator.

    public static TheoryData<decimal, decimal> CelsiusToFahrenheitCases => new()
    {
        { 0m, 32m },             // water freezes
        { 100m, 212m },          // water boils
        { -40m, -40m },          // the one point where both scales agree
        { 37m, 98.6m },          // normal body temperature
        { -273.15m, -459.67m },  // absolute zero is allowed, just not below it
    };

    public static TheoryData<decimal, decimal> FahrenheitToCelsiusCases => new()
    {
        { 32m, 0m },
        { 212m, 100m },
        { -40m, -40m },
        { 98.6m, 37m },
        { -459.67m, -273.15m },
    };


    // ---------- Conversions ----------

    [Theory]
    [MemberData(nameof(CelsiusToFahrenheitCases))]
    public void CelsiusToFahrenheit_ReturnsExpectedValue(decimal celsius, decimal expectedFahrenheit)
    {
        Assert.Equal(expectedFahrenheit, TemperatureConverter.CelsiusToFahrenheit(celsius));
    }

    [Theory]
    [MemberData(nameof(FahrenheitToCelsiusCases))]
    public void FahrenheitToCelsius_ReturnsExpectedValue(decimal fahrenheit, decimal expectedCelsius)
    {
        Assert.Equal(expectedCelsius, TemperatureConverter.FahrenheitToCelsius(fahrenheit));
    }

    [Fact]
    public void FahrenheitToCelsius_RepeatingDecimal_RoundsSensibly()
    {
        // 100 F is 37.777... C, a repeating decimal.
        // We check it rounds to the value a person would expect to see.
        var celsius = TemperatureConverter.FahrenheitToCelsius(100m);

        Assert.Equal(37.78m, Math.Round(celsius, 2));
    }


    // ---------- Invalid input ----------

    [Fact]
    public void CelsiusToFahrenheit_BelowAbsoluteZero_ThrowsWithClearMessage()
    {
        var ex = Assert.Throws<ArgumentException>(() => TemperatureConverter.CelsiusToFahrenheit(-273.16m));

        Assert.Equal("Temperature cannot be below absolute zero (-273.15 C).", ex.Message);
    }

    [Fact]
    public void FahrenheitToCelsius_BelowAbsoluteZero_ThrowsWithClearMessage()
    {
        var ex = Assert.Throws<ArgumentException>(() => TemperatureConverter.FahrenheitToCelsius(-459.68m));

        Assert.Equal("Temperature cannot be below absolute zero (-459.67 F).", ex.Message);
    }
}