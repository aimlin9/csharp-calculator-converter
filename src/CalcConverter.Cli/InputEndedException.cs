namespace CalcConverter.Cli;

/// <summary>
/// Signals that no more input will ever arrive.
/// </summary>
/// <remarks>
/// This happens when the user presses Ctrl+Z then Enter on Windows (Ctrl+D on
/// Linux or macOS), or when input is piped in from a file and the file runs out.
///
/// Without this, Console.ReadLine() would keep returning null and the
/// "ask again" loop would spin forever. Throwing a dedicated exception lets
/// the app notice it and exit cleanly instead.
/// </remarks>
public sealed class InputEndedException : Exception
{
    public InputEndedException()
        : base("Input ended.")
    {
    }
}