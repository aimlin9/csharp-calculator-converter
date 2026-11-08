using System.Globalization;

namespace CalcConverter.Core;

/// <summary>
/// Turns text typed by a user into numbers, without ever throwing an exception.
/// </summary>
/// <remarks>
/// Number rules:
/// - Spaces before or after the number are ignored.
/// - A leading + or - sign is allowed (for example -40).
/// - The decimal point must be a dot (15.5). Commas are rejected.
///
/// Why reject commas?
/// "1,500" could mean fifteen hundred (comma as thousands separator) or one and
/// a half (comma as decimal point, used in many countries). Guessing wrong would
/// give a silently wrong answer, which is worse than asking the user to retype.
///
/// Why InvariantCulture?
/// Without it, .NET follows the computer's regional settings, so the same input
/// could be read differently on different machines. InvariantCulture makes the
/// rules identical everywhere.
///
/// This class lives in Core rather than the console project because parsing
/// text has nothing to do with the console itself, and here it can be unit tested.
/// </remarks>
public static class InputParser
{
    // Only the number formats we deliberately support. Anything else is rejected.
    private const NumberStyles NumberRules =
        NumberStyles.AllowLeadingWhite |
        NumberStyles.AllowTrailingWhite |
        NumberStyles.AllowLeadingSign |
        NumberStyles.AllowDecimalPoint;

    // Menu choices are whole, positive numbers, so no sign or decimal point.
    private const NumberStyles MenuRules =
        NumberStyles.AllowLeadingWhite |
        NumberStyles.AllowTrailingWhite;


    /// <summary>
    /// Tries to read a decimal number from user input.
    /// </summary>
    /// <returns>
    /// True if the input is a valid number; false for empty, malformed,
    /// or too-large input.
    /// </returns>
    public static bool TryParseNumber(string? input, out decimal value)
    {
        // decimal.TryParse already copes with null, empty text and numbers too
        // large for decimal. It simply returns false instead of throwing.
        return decimal.TryParse(input, NumberRules, CultureInfo.InvariantCulture, out value);
    }


    /// <summary>
    /// Tries to read a menu choice that must fall between min and max (inclusive).
    /// </summary>
    public static bool TryParseMenuChoice(string? input, int min, int max, out int choice)
    {
        if (int.TryParse(input, MenuRules, CultureInfo.InvariantCulture, out choice)
            && choice >= min
            && choice <= max)
        {
            return true;
        }

        // Reset so callers never act on a half-valid value by accident.
        choice = 0;
        return false;
    }
}