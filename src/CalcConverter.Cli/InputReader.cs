using CalcConverter.Core;

namespace CalcConverter.Cli;

/// <summary>
/// Asks the user for input and keeps asking until it gets something valid.
/// </summary>
/// <remarks>
/// This is where the "handle wrong input without crashing" requirement is met
/// on the console side. Bad input never throws. The user sees a short, friendly
/// hint and is simply asked again.
///
/// The reader and writer are passed in rather than using Console directly.
/// In the real app they are Console.In and Console.Out; in tests they are
/// in-memory text, so we can script a whole conversation and check the replies.
/// </remarks>
public sealed class InputReader
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public InputReader(TextReader input, TextWriter output)
    {
        _input = input;
        _output = output;
    }


    /// <summary>
    /// Shows the prompt and returns the first valid number the user enters.
    /// </summary>
    public decimal ReadNumber(string prompt)
    {
        while (true)
        {
            var text = ReadLineOrThrow(prompt);

            if (InputParser.TryParseNumber(text, out var value))
            {
                return value;
            }

            _output.WriteLine("  That is not a valid number. Use digits and a dot for decimals, for example 15.5");
        }
    }


    /// <summary>
    /// Shows the prompt and returns the first menu choice between min and max.
    /// </summary>
    public int ReadMenuChoice(string prompt, int min, int max)
    {
        while (true)
        {
            var text = ReadLineOrThrow(prompt);

            if (InputParser.TryParseMenuChoice(text, min, max, out var choice))
            {
                return choice;
            }

            _output.WriteLine($"  Please enter a number from {min} to {max}.");
        }
    }


    // Writes the prompt on the same line as the user's answer, then reads it.
    // A null line means the input stream has closed for good (see InputEndedException).
    private string ReadLineOrThrow(string prompt)
    {
        _output.Write(prompt);

        var line = _input.ReadLine();

        if (line is null)
        {
            throw new InputEndedException();
        }

        return line;
    }
}