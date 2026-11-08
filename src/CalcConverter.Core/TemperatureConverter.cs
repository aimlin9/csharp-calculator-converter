namespace CalcConverter.Core;

/// <summary>
/// Converts temperatures between Celsius and Fahrenheit.
/// </summary>
/// <remarks>
/// Formulas:
///   F = C x 9/5 + 32
///   C = (F - 32) x 5/9
///
/// Unlike distance, temperature can be negative, so we cannot simply reject
/// values below zero. The true physical floor is absolute zero
/// (-273.15 C, which is -459.67 F). Nothing can be colder, so anything below
/// that is treated as a typing mistake and rejected with a clear message.
/// </remarks>
public static class TemperatureConverter
{
    /// <summary>Absolute zero on the Celsius scale.</summary>
    public const decimal AbsoluteZeroCelsius = -273.15m;

    /// <summary>Absolute zero on the Fahrenheit scale.</summary>
    public const decimal AbsoluteZeroFahrenheit = -459.67m;


    /// <summary>Converts Celsius to Fahrenheit.</summary>
    /// <exception cref="ArgumentException">Thrown when the value is below absolute zero.</exception>
    public static decimal CelsiusToFahrenheit(decimal celsius)
    {
        if (celsius < AbsoluteZeroCelsius)
        {
            throw new ArgumentException("Temperature cannot be below absolute zero (-273.15 C).");
        }

        return celsius * 9m / 5m + 32m;
    }


    /// <summary>Converts Fahrenheit to Celsius.</summary>
    /// <exception cref="ArgumentException">Thrown when the value is below absolute zero.</exception>
    public static decimal FahrenheitToCelsius(decimal fahrenheit)
    {
        if (fahrenheit < AbsoluteZeroFahrenheit)
        {
            throw new ArgumentException("Temperature cannot be below absolute zero (-459.67 F).");
        }

        return (fahrenheit - 32m) * 5m / 9m;
    }
}