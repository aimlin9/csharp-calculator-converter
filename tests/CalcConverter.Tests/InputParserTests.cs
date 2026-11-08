using CalcConverter.Core;

namespace CalcConverter.Tests;

/// <summary>
/// Unit tests for <see cref="InputParser"/>.
/// These guard the "never crash on bad input" requirement at its source.
/// </summary>
public class InputParserTests
{
    // ---------- Valid numbers ----------

    public static TheoryData<string, decimal> ValidNumbers => new()
    {
        { "42", 42m },
        { "-40", -40m },          // negative temperatures must work
        { "15.5", 15.5m },
        { "  7.25  ", 7.25m },    // stray spaces are forgiven
        { "+3", 3m },
        { "0.1", 0.1m },
    };

    [Theory]
    [MemberData(nameof(ValidNumbers))]
    public void TryParseNumber_ValidInput_ReturnsTrueAndValue(string input, decimal expected)
    {
        var success = InputParser.TryParseNumber(input, out var value);

        Assert.True(success);
        Assert.Equal(expected, value);
    }


    // ---------- Invalid numbers ----------

    [Theory]
    [InlineData(null)]                                // input stream closed
    [InlineData("")]                                  // user just pressed Enter
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12abc")]
    [InlineData("1,5")]                               // comma rejected on purpose
    [InlineData("1,000")]
    [InlineData("1.2.3")]
    [InlineData("--5")]
    [InlineData("1e5")]                               // scientific notation not supported
    [InlineData("99999999999999999999999999999999")]  // too large for decimal
    public void TryParseNumber_InvalidInput_ReturnsFalse(string? input)
    {
        Assert.False(InputParser.TryParseNumber(input, out _));
    }


    // ---------- Menu choices ----------

    [Theory]
    [InlineData("1", 1)]
    [InlineData("5", 5)]
    [InlineData(" 3 ", 3)]
    public void TryParseMenuChoice_InRange_ReturnsTrueAndChoice(string input, int expected)
    {
        var success = InputParser.TryParseMenuChoice(input, 1, 5, out var choice);

        Assert.True(success);
        Assert.Equal(expected, choice);
    }

    [Theory]
    [InlineData("0")]      // just below the range
    [InlineData("6")]      // just above the range
    [InlineData("-1")]
    [InlineData("2.5")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParseMenuChoice_OutOfRangeOrInvalid_ReturnsFalseAndZero(string? input)
    {
        var success = InputParser.TryParseMenuChoice(input, 1, 5, out var choice);

        Assert.False(success);
        Assert.Equal(0, choice);
    }
}