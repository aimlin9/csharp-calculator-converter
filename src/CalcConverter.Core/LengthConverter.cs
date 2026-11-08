namespace CalcConverter.Core;

/// <summary>
/// Converts distances between kilometres and miles.
/// </summary>
/// <remarks>
/// The international mile is defined as exactly 1.609344 km. We store that
/// one constant and build both directions from it, so the two conversions
/// can never drift out of step with each other.
///
/// A distance cannot be negative, so negative input is rejected instead of
/// quietly producing a meaningless answer.
/// </remarks>
public static class LengthConverter
{
    /// <summary>Exact number of kilometres in one international mile.</summary>
    public const decimal KilometresPerMile = 1.609344m;


    /// <summary>Converts kilometres to miles.</summary>
    /// <exception cref="ArgumentException">Thrown when the distance is negative.</exception>
    public static decimal KilometresToMiles(decimal kilometres)
    {
        EnsureNotNegative(kilometres);

        return kilometres / KilometresPerMile;
    }


    /// <summary>Converts miles to kilometres.</summary>
    /// <exception cref="ArgumentException">Thrown when the distance is negative.</exception>
    public static decimal MilesToKilometres(decimal miles)
    {
        EnsureNotNegative(miles);

        return miles * KilometresPerMile;
    }


    // Shared guard so both directions apply exactly the same rule
    // and show the user exactly the same message.
    private static void EnsureNotNegative(decimal distance)
    {
        if (distance < 0)
        {
            throw new ArgumentException("Distance cannot be negative.");
        }
    }
}