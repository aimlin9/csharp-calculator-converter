using CalcConverter.Cli;

namespace CalcConverter.Tests;

/// <summary>
/// End-to-end tests that run whole user sessions through <see cref="App"/>.
/// </summary>
/// <remarks>
/// Each test lists exactly what a user would type, line by line, then checks
/// what the app printed. Most sessions end with "5" (Exit), which also proves
/// the app returned to the main menu after the screen finished, even after an error.
/// </remarks>
public class AppTests
{
    private static (int ExitCode, string Output) RunApp(params string[] lines)
    {
        var input = new StringReader(string.Join(Environment.NewLine, lines));
        var output = new StringWriter();

        var exitCode = new App(input, output).Run();

        return (exitCode, output.ToString());
    }


    // ---------- Happy paths ----------

    [Fact]
    public void Exit_FromMainMenu_EndsCleanly()
    {
        var (exitCode, output) = RunApp("5");

        Assert.Equal(0, exitCode);
        Assert.Contains("Goodbye!", output);
    }

    [Fact]
    public void Calculator_Add_ShowsResult()
    {
        // Menu 1 (Calculator), first 6, operation 1 (Add), second 2, then Exit.
        var (_, output) = RunApp("1", "6", "1", "2", "5");

        Assert.Contains("Result: 6 + 2 = 8", output);
    }

    [Fact]
    public void Length_KilometresToMiles_ShowsResult()
    {
        var (_, output) = RunApp("2", "1", "10", "5");

        Assert.Contains("Result: 10 km = 6.2137 miles", output);
    }

    [Fact]
    public void Length_MilesToKilometres_ShowsResult()
    {
        var (_, output) = RunApp("2", "2", "5", "5");

        Assert.Contains("Result: 5 miles = 8.0467 km", output);
    }

    [Fact]
    public void Temperature_CelsiusToFahrenheit_ShowsResult()
    {
        var (_, output) = RunApp("3", "1", "100", "5");

        Assert.Contains("Result: 100 C = 212 F", output);
    }

    [Fact]
    public void Currency_GhsToUsd_ShowsResultToTheCent()
    {
        var (_, output) = RunApp("4", "100", "15.5", "5");

        Assert.Contains("GHS 100.00 = USD 6.45", output);
    }


    // ---------- Errors are reported, and the app carries on ----------

    [Fact]
    public void Calculator_DivideByZero_ShowsErrorAndReturnsToMenu()
    {
        var (exitCode, output) = RunApp("1", "5", "4", "0", "5");

        Assert.Contains("Error: Cannot divide by zero.", output);
        Assert.Contains("Goodbye!", output);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public void Calculator_Overflow_ShowsFriendlyError()
    {
        // decimal.MaxValue multiplied by 2 cannot fit in a decimal.
        var (_, output) = RunApp("1", "79228162514264337593543950335", "3", "2", "5");

        Assert.Contains("Error: The result is too large to calculate.", output);
    }

    [Fact]
    public void Temperature_BelowAbsoluteZero_ShowsError()
    {
        var (_, output) = RunApp("3", "1", "-300", "5");

        Assert.Contains("Error: Temperature cannot be below absolute zero (-273.15 C).", output);
    }

    [Fact]
    public void Currency_ZeroRate_ShowsError()
    {
        var (_, output) = RunApp("4", "100", "0", "5");

        Assert.Contains("Error: Exchange rate must be greater than zero.", output);
    }


    // ---------- Never crashes ----------

    [Fact]
    public void GarbageInput_EverywhereThenValid_StillCompletes()
    {
        // "abc" and "9" at the menu, "x" as a number: each is rejected and re-asked,
        // then the session continues normally.
        var (exitCode, output) = RunApp("abc", "9", "1", "x", "6", "1", "2", "5");

        Assert.Contains("Result: 6 + 2 = 8", output);
        Assert.Equal(0, exitCode);
    }

    [Fact]
    public void InputEndsMidSession_ExitsCleanly()
    {
        // The user starts a calculation, then the input stream closes.
        var (exitCode, output) = RunApp("1", "6");

        Assert.Equal(0, exitCode);
        Assert.Contains("Input ended. Goodbye!", output);
    }
}