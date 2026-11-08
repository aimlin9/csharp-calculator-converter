using CalcConverter.Cli;

namespace CalcConverter.Tests;

/// <summary>
/// Tests for <see cref="InputReader"/>, using in-memory text instead of a keyboard.
/// </summary>
public class InputReaderTests
{
    // Builds a reader fed by the given lines, and captures everything it prints.
    private static (InputReader Reader, StringWriter Output) CreateReader(params string[] lines)
    {
        var input = new StringReader(string.Join(Environment.NewLine, lines));
        var output = new StringWriter();

        return (new InputReader(input, output), output);
    }


    [Fact]
    public void ReadNumber_ValidOnFirstTry_ReturnsValue()
    {
        var (reader, _) = CreateReader("42.5");

        Assert.Equal(42.5m, reader.ReadNumber("Number: "));
    }

    [Fact]
    public void ReadNumber_InvalidThenValid_ShowsHintAndAsksAgain()
    {
        var (reader, output) = CreateReader("abc", "12.5");

        var value = reader.ReadNumber("Number: ");

        Assert.Equal(12.5m, value);
        Assert.Contains("That is not a valid number", output.ToString());
    }

    [Fact]
    public void ReadMenuChoice_OutOfRangeThenValid_ShowsRangeAndAsksAgain()
    {
        var (reader, output) = CreateReader("9", "2");

        var choice = reader.ReadMenuChoice("Choice: ", 1, 4);

        Assert.Equal(2, choice);
        Assert.Contains("Please enter a number from 1 to 4.", output.ToString());
    }

    [Fact]
    public void ReadNumber_InputEnds_ThrowsInputEndedException()
    {
        // No lines at all: the stream is closed before the user answers.
        var (reader, _) = CreateReader();

        Assert.Throws<InputEndedException>(() => reader.ReadNumber("Number: "));
    }
}