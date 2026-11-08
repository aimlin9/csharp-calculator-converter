using System.Globalization;

using CalcConverter.Core;

namespace CalcConverter.Cli;

/// <summary>
/// The console application: shows the menu, runs each screen, and reports errors.
/// </summary>
/// <remarks>
/// How responsibilities are split:
/// - Core does all the math and decides what input is allowed.
/// - InputReader makes sure we only ever receive well-formed numbers.
/// - App (this class) connects the two and formats results for people to read.
///
/// Error handling strategy:
/// Each screen runs inside a single try/catch (see RunScreen). If Core rejects a
/// value, for example dividing by zero or a negative distance, the user sees
/// Core's message and returns to the main menu. Nothing reaches the top level
/// as an unhandled crash.
/// </remarks>
public sealed class App
{
    private readonly TextWriter _output;
    private readonly InputReader _reader;
    private readonly Calculator _calculator = new();

    public App(TextReader input, TextWriter output)
    {
        _output = output;
        _reader = new InputReader(input, output);
    }


    /// <summary>
    /// Runs the app until the user chooses Exit or the input stream ends.
    /// </summary>
    /// <returns>The process exit code. 0 means a normal, clean exit.</returns>
    public int Run()
    {
        WriteBanner();

        try
        {
            while (true)
            {
                WriteMainMenu();

                var choice = _reader.ReadMenuChoice("Choose an option (1-5): ", 1, 5);

                if (choice == 5)
                {
                    _output.WriteLine("Goodbye!");
                    return 0;
                }

                RunScreen(choice);
            }
        }
        catch (InputEndedException)
        {
            // The user closed the input (Ctrl+Z) or piped input ran out.
            // Leave politely instead of crashing or looping forever.
            _output.WriteLine();
            _output.WriteLine("Input ended. Goodbye!");
            return 0;
        }
    }


    // ---------- Screen routing and error handling ----------

    private void RunScreen(int choice)
    {
        _output.WriteLine();

        try
        {
            switch (choice)
            {
                case 1: RunCalculator(); break;
                case 2: RunLengthConverter(); break;
                case 3: RunTemperatureConverter(); break;
                case 4: RunCurrencyConverter(); break;
            }
        }
        catch (ArgumentException ex)
        {
            // Core's validation messages are written for users, so show them as-is.
            WriteError(ex.Message);
        }
        catch (DivideByZeroException ex)
        {
            WriteError(ex.Message);
        }
        catch (OverflowException)
        {
            // decimal tops out around 7.9 x 10^28. Explain it in plain words.
            WriteError("The result is too large to calculate. Please try smaller numbers.");
        }

        _output.WriteLine();
    }


    // ---------- Screens ----------

    private void RunCalculator()
    {
        WriteHeading("Calculator");

        var first = _reader.ReadNumber("First number: ");

        _output.WriteLine("  1. Add (+)");
        _output.WriteLine("  2. Subtract (-)");
        _output.WriteLine("  3. Multiply (*)");
        _output.WriteLine("  4. Divide (/)");
        var choice = _reader.ReadMenuChoice("Operation (1-4): ", 1, 4);

        var second = _reader.ReadNumber("Second number: ");

        // Map menu numbers to operations explicitly, rather than relying on
        // the enum's internal order, so reordering the enum can never break this.
        var operation = choice switch
        {
            1 => Operation.Add,
            2 => Operation.Subtract,
            3 => Operation.Multiply,
            _ => Operation.Divide,
        };

        var result = _calculator.Calculate(first, operation, second);

        WriteResult($"{Format(first, 10)} {SymbolFor(operation)} {Format(second, 10)} = {Format(result, 10)}");
    }


    private void RunLengthConverter()
    {
        WriteHeading("Kilometres and miles");

        _output.WriteLine("  1. Kilometres to miles");
        _output.WriteLine("  2. Miles to kilometres");
        var direction = _reader.ReadMenuChoice("Direction (1-2): ", 1, 2);

        if (direction == 1)
        {
            var kilometres = _reader.ReadNumber("Distance in kilometres: ");
            var miles = LengthConverter.KilometresToMiles(kilometres);

            WriteResult($"{Format(kilometres, 4)} km = {Format(miles, 4)} miles");
        }
        else
        {
            var miles = _reader.ReadNumber("Distance in miles: ");
            var kilometres = LengthConverter.MilesToKilometres(miles);

            WriteResult($"{Format(miles, 4)} miles = {Format(kilometres, 4)} km");
        }
    }


    private void RunTemperatureConverter()
    {
        WriteHeading("Celsius and Fahrenheit");

        _output.WriteLine("  1. Celsius to Fahrenheit");
        _output.WriteLine("  2. Fahrenheit to Celsius");
        var direction = _reader.ReadMenuChoice("Direction (1-2): ", 1, 2);

        if (direction == 1)
        {
            var celsius = _reader.ReadNumber("Temperature in Celsius: ");
            var fahrenheit = TemperatureConverter.CelsiusToFahrenheit(celsius);

            WriteResult($"{Format(celsius, 2)} C = {Format(fahrenheit, 2)} F");
        }
        else
        {
            var fahrenheit = _reader.ReadNumber("Temperature in Fahrenheit: ");
            var celsius = TemperatureConverter.FahrenheitToCelsius(fahrenheit);

            WriteResult($"{Format(fahrenheit, 2)} F = {Format(celsius, 2)} C");
        }
    }


    private void RunCurrencyConverter()
    {
        WriteHeading("Ghana cedis to US dollars");

        var amount = _reader.ReadNumber("Amount in GHS: ");

        // Spell out what the rate means, so nobody types it upside down
        // (0.065 dollars per cedi instead of 15.5 cedis per dollar).
        var rate = _reader.ReadNumber("How many GHS equal 1 USD today? (for example 15.50): ");

        var usd = CurrencyConverter.GhsToUsd(amount, rate);

        WriteResult($"GHS {FormatMoney(amount)} = USD {FormatMoney(usd)}  (at {Format(rate, 4)} GHS per USD)");
    }


    // ---------- Output helpers ----------

    private void WriteBanner()
    {
        _output.WriteLine("=================================");
        _output.WriteLine("  Calculator and Unit Converter");
        _output.WriteLine("=================================");
        _output.WriteLine();
    }

    private void WriteMainMenu()
    {
        _output.WriteLine("Main menu");
        _output.WriteLine("  1. Calculator");
        _output.WriteLine("  2. Kilometres and miles");
        _output.WriteLine("  3. Celsius and Fahrenheit");
        _output.WriteLine("  4. Ghana cedis to US dollars");
        _output.WriteLine("  5. Exit");
    }

    private void WriteHeading(string title) => _output.WriteLine($"--- {title} ---");

    private void WriteResult(string text) => _output.WriteLine($"  Result: {text}");

    private void WriteError(string message) => _output.WriteLine($"  Error: {message}");


    // ---------- Formatting ----------

    /// <summary>
    /// Rounds for display and drops trailing zeros (8.50 shows as 8.5).
    /// </summary>
    /// <remarks>
    /// Rounding happens only here, at the edge where people read the number.
    /// Core always works at full precision.
    /// AwayFromZero rounds 2.5 up to 3, which is what most people expect.
    /// (.NET's default rounds 2.5 to 2, called banker's rounding.)
    /// </remarks>
    private static string Format(decimal value, int maxDecimals)
    {
        var rounded = Math.Round(value, maxDecimals, MidpointRounding.AwayFromZero);

        // A tiny negative number can round to "-0". Show plain 0 instead.
        if (rounded == 0)
        {
            rounded = 0m;
        }

        var pattern = "0." + new string('#', maxDecimals);
        return rounded.ToString(pattern, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Formats money to exactly two decimal places with thousands separators (1,234.50).
    /// </summary>
    private static string FormatMoney(decimal value)
    {
        var rounded = Math.Round(value, 2, MidpointRounding.AwayFromZero);
        return rounded.ToString("#,0.00", CultureInfo.InvariantCulture);
    }

    private static string SymbolFor(Operation operation) => operation switch
    {
        Operation.Add => "+",
        Operation.Subtract => "-",
        Operation.Multiply => "*",
        _ => "/",
    };
}