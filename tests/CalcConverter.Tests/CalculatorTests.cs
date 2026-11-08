using CalcConverter.Core;

namespace CalcConverter.Tests;

/// <summary>
/// Unit tests for <see cref="Calculator"/>.
/// Each test follows the pattern: arrange the inputs, act on the calculator,
/// then assert the result is what a person would expect.
/// </summary>
public class CalculatorTests
{
    // Calculator holds no state, so one shared instance is safe for every test.
    private readonly Calculator _calculator = new();


    // ---------- Test data ----------
    // xUnit attributes cannot hold decimal values directly,
    // so we supply decimal cases through TheoryData instead.

    public static TheoryData<decimal, decimal, decimal> AddCases => new()
    {
        { 2m, 3m, 5m },
        { -4m, 10m, 6m },
        { 0.1m, 0.2m, 0.3m },   // the classic floating-point trap, exact with decimal
    };

    public static TheoryData<decimal, decimal, decimal> SubtractCases => new()
    {
        { 10m, 4m, 6m },
        { 4m, 10m, -6m },
        { 0m, 0m, 0m },
    };

    public static TheoryData<decimal, decimal, decimal> MultiplyCases => new()
    {
        { 3m, 4m, 12m },
        { -2m, 5m, -10m },
        { 7m, 0m, 0m },
    };

    public static TheoryData<decimal, decimal, decimal> DivideCases => new()
    {
        { 10m, 2m, 5m },
        { -9m, 3m, -3m },
        { 1m, 4m, 0.25m },
    };


    // ---------- Basic operations ----------

    [Theory]
    [MemberData(nameof(AddCases))]
    public void Add_ReturnsSum(decimal a, decimal b, decimal expected)
    {
        Assert.Equal(expected, _calculator.Add(a, b));
    }

    [Theory]
    [MemberData(nameof(SubtractCases))]
    public void Subtract_ReturnsDifference(decimal a, decimal b, decimal expected)
    {
        Assert.Equal(expected, _calculator.Subtract(a, b));
    }

    [Theory]
    [MemberData(nameof(MultiplyCases))]
    public void Multiply_ReturnsProduct(decimal a, decimal b, decimal expected)
    {
        Assert.Equal(expected, _calculator.Multiply(a, b));
    }

    [Theory]
    [MemberData(nameof(DivideCases))]
    public void Divide_ReturnsQuotient(decimal a, decimal b, decimal expected)
    {
        Assert.Equal(expected, _calculator.Divide(a, b));
    }


    // ---------- Edge cases ----------

    [Fact]
    public void Divide_ByZero_ThrowsWithClearMessage()
    {
        var ex = Assert.Throws<DivideByZeroException>(() => _calculator.Divide(5m, 0m));

        Assert.Equal("Cannot divide by zero.", ex.Message);
    }

    [Fact]
    public void Multiply_BeyondDecimalRange_ThrowsOverflow()
    {
        // decimal.MaxValue is about 7.9 x 10^28; doubling it cannot fit.
        Assert.Throws<OverflowException>(() => _calculator.Multiply(decimal.MaxValue, 2m));
    }


    // ---------- Calculate dispatcher ----------

    [Theory]
    [InlineData(Operation.Add, 8)]
    [InlineData(Operation.Subtract, 4)]
    [InlineData(Operation.Multiply, 12)]
    [InlineData(Operation.Divide, 3)]
    public void Calculate_RoutesToCorrectOperation(Operation operation, int expected)
    {
        // 6 and 2 give a different answer for every operation,
        // so a wrong route would fail this test.
        Assert.Equal(expected, _calculator.Calculate(6m, operation, 2m));
    }

    [Fact]
    public void Calculate_WithInvalidOperation_Throws()
    {
        var invalid = (Operation)99;

        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.Calculate(1m, invalid, 1m));
    }
}