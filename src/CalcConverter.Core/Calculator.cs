namespace CalcConverter.Core;

/// <summary>
/// Performs basic arithmetic on two numbers.
/// </summary>
/// <remarks>
/// Why decimal and not double?
/// decimal stores base-10 numbers exactly, so 0.1 + 0.2 gives 0.3, which is
/// what someone using a calculator expects. double would give 0.30000000000000004.
/// The trade-off is a smaller range (roughly 7.9 x 10^28), so extremely large
/// results throw an OverflowException. The console layer will catch that and
/// explain it to the user instead of crashing.
///
/// This class knows nothing about the console. It only does math, which keeps
/// it simple to test and easy to reuse in another interface later.
/// </remarks>
public class Calculator
{
    /// <summary>Returns the sum of two numbers.</summary>
    public decimal Add(decimal a, decimal b) => a + b;

    /// <summary>Returns the result of subtracting <paramref name="b"/> from <paramref name="a"/>.</summary>
    public decimal Subtract(decimal a, decimal b) => a - b;

    /// <summary>Returns the product of two numbers.</summary>
    public decimal Multiply(decimal a, decimal b) => a * b;

    /// <summary>Divides <paramref name="dividend"/> by <paramref name="divisor"/>.</summary>
    /// <exception cref="DivideByZeroException">Thrown when the divisor is zero.</exception>
    public decimal Divide(decimal dividend, decimal divisor)
    {
        // Dividing by zero has no meaningful answer.
        // We check for it ourselves so the error message is clear and friendly.
        if (divisor == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero.");
        }

        return dividend / divisor;
    }

    /// <summary>
    /// Runs the given operation on two numbers.
    /// </summary>
    /// <remarks>
    /// This gives the console layer a single entry point: it works out which
    /// operation the user picked, then hands everything over here.
    /// </remarks>
    public decimal Calculate(decimal a, Operation operation, decimal b) => operation switch
    {
        Operation.Add => Add(a, b),
        Operation.Subtract => Subtract(a, b),
        Operation.Multiply => Multiply(a, b),
        Operation.Divide => Divide(a, b),

        // Only reachable if someone casts an invalid number to Operation.
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, "Unsupported operation.")
    };
}