namespace CalcConverter.Core;

/// <summary>
/// Converts Ghana cedis (GHS) to US dollars (USD) using a rate the user provides.
/// </summary>
/// <remarks>
/// How the rate is read:
/// The rate means "how many cedis buy one US dollar", for example 15.50.
/// That is how the rate is usually quoted at banks and forex bureaus in Ghana,
/// so it matches what a user will naturally type in.
///
///   USD = GHS / rate
///
/// Why the user supplies the rate:
/// Exchange rates change daily. Hard-coding one would go stale quickly, and
/// fetching one live would add a network dependency the task does not ask for.
///
/// Validation:
/// - The amount cannot be negative.
/// - The rate must be greater than zero. A zero rate would cause division by zero,
///   and a negative rate has no real-world meaning.
/// </remarks>
public static class CurrencyConverter
{
    /// <summary>Converts an amount in cedis to US dollars.</summary>
    /// <param name="amountGhs">The amount in Ghana cedis.</param>
    /// <param name="ghsPerUsd">How many cedis equal one US dollar.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the amount is negative or the rate is zero or negative.
    /// </exception>
    public static decimal GhsToUsd(decimal amountGhs, decimal ghsPerUsd)
    {
        if (amountGhs < 0)
        {
            throw new ArgumentException("Amount cannot be negative.");
        }

        if (ghsPerUsd <= 0)
        {
            throw new ArgumentException("Exchange rate must be greater than zero.");
        }

        return amountGhs / ghsPerUsd;
    }
}