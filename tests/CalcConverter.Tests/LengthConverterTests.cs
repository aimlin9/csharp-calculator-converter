using CalcConverter.Core;

namespace CalcConverter.Tests;

/// <summary>
/// Unit tests for <see cref="LengthConverter"/>.
/// </summary>
public class LengthConverterTests
{
    // ---------- Test data ----------

    // Kilometres to miles involves division, so results can have many decimal
    // places. We compare after rounding to 4 places, which is far more precise
    // than anyone needs for a distance.
    public static TheoryData<decimal, decimal> KilometresToMilesCases => new()
    {
        { 1.609344m, 1m },        // exactly one mile
        { 0m, 0m },
        { 10m, 6.2137m },
        { 42.195m, 26.2188m },    // a marathon
    };

    // Miles to kilometres is a plain multiplication by an exact constant,
    // so these results are exact and need no rounding.
    public static TheoryData<decimal, decimal> MilesToKilometresCases => new()
    {
        { 1m, 1.609344m },
        { 0m, 0m },
        { 5m, 8.04672m },
        { 26.2m, 42.1648128m },
    };


    // ---------- Conversions ----------

    [Theory]
    [MemberData(nameof(KilometresToMilesCases))]
    public void KilometresToMiles_ReturnsExpectedMiles(decimal kilometres, decimal expectedMiles)
    {
        var miles = LengthConverter.KilometresToMiles(kilometres);

        Assert.Equal(expectedMiles, Math.Round(miles, 4));
    }

    [Theory]
    [MemberData(nameof(MilesToKilometresCases))]
    public void MilesToKilometres_ReturnsExpectedKilometres(decimal miles, decimal expectedKilometres)
    {
        Assert.Equal(expectedKilometres, LengthConverter.MilesToKilometres(miles));
    }

    [Fact]
    public void RoundTrip_KilometresToMilesAndBack_ReturnsOriginalValue()
    {
        // Converting there and back should land where we started.
        // This catches mismatched formulas that single-direction tests could miss.
        const decimal original = 123.45m;

        var miles = LengthConverter.KilometresToMiles(original);
        var backToKilometres = LengthConverter.MilesToKilometres(miles);

        Assert.Equal(original, Math.Round(backToKilometres, 10));
    }


    // ---------- Invalid input ----------

    [Fact]
    public void KilometresToMiles_Negative_ThrowsWithClearMessage()
    {
        var ex = Assert.Throws<ArgumentException>(() => LengthConverter.KilometresToMiles(-1m));

        Assert.Equal("Distance cannot be negative.", ex.Message);
    }

    [Fact]
    public void MilesToKilometres_Negative_Throws()
    {
        Assert.Throws<ArgumentException>(() => LengthConverter.MilesToKilometres(-0.5m));
    }
}