namespace CalcConverter.Core;

/// <summary>
/// The four arithmetic operations the calculator supports.
/// </summary>
/// <remarks>
/// Using an enum instead of raw strings like "+" or "add" means the compiler
/// catches typos for us, and the console layer has one clear list to map
/// user choices onto.
/// </remarks>
public enum Operation
{
    Add,
    Subtract,
    Multiply,
    Divide
}