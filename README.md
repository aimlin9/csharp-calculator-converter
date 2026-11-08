# Calculator and Unit Converter (C#)

![CI](https://github.com/aimlin9/csharp-calculator-converter/actions/workflows/ci.yml/badge.svg)

A console calculator and unit converter built with C# and .NET 10.
It handles wrong input without crashing and is covered by 95 automated tests.

## Features

- **Calculator:** add, subtract, multiply and divide two numbers
- **Length:** kilometres to miles, and miles to kilometres
- **Temperature:** Celsius to Fahrenheit, and Fahrenheit to Celsius
- **Currency:** Ghana cedis (GHS) to US dollars (USD), using a rate you type in
- **Safe input:** invalid entries are rejected with a clear hint and asked again; impossible values (dividing by zero, negative distances, temperatures below absolute zero, a zero exchange rate) show a friendly error and return you to the menu

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (built and tested with 10.0.403)
- Git

Check your SDK version with:

```bash
dotnet --version
```

## Getting started

Clone the repository:

```bash
git clone https://github.com/aimlin9/csharp-calculator-converter.git
cd csharp-calculator-converter
```

Run the app:

```bash
dotnet run --project src/CalcConverter.Cli
```

Run the tests:

```bash
dotnet test
```

To use Visual Studio instead, open `CalcConverter.slnx`, set **CalcConverter.Cli** as the startup project and press **F5**. Run the tests from **Test > Test Explorer > Run All**.

## Example session

```text
=================================
  Calculator and Unit Converter
=================================

Main menu
  1. Calculator
  2. Kilometres and miles
  3. Celsius and Fahrenheit
  4. Ghana cedis to US dollars
  5. Exit
Choose an option (1-5): 4

--- Ghana cedis to US dollars ---
Amount in GHS: 100
How many GHS equal 1 USD today? (for example 15.50): 15.5
  Result: GHS 100.00 = USD 6.45  (at 15.5 GHS per USD)

Main menu
  ...
Choose an option (1-5): 1

--- Calculator ---
First number: 10
  1. Add (+)
  2. Subtract (-)
  3. Multiply (*)
  4. Divide (/)
Operation (1-4): 4
Second number: 0
  Error: Cannot divide by zero.
```

## Project structure

```text
csharp-calculator-converter/
├── src/
│   ├── CalcConverter.Core/        All logic: math, conversions, input parsing
│   │   ├── Calculator.cs
│   │   ├── Operation.cs
│   │   ├── LengthConverter.cs
│   │   ├── TemperatureConverter.cs
│   │   ├── CurrencyConverter.cs
│   │   └── InputParser.cs
│   └── CalcConverter.Cli/         The console app: menus, prompts, output
│       ├── Program.cs
│       ├── App.cs
│       ├── InputReader.cs
│       └── InputEndedException.cs
├── tests/
│   └── CalcConverter.Tests/       xUnit tests for Core and the console app
├── docs/
│   └── DESIGN.md                  Architecture and engineering decisions
└── .github/workflows/ci.yml       Builds and tests on every push and PR
```

## Architecture in brief

The solution is split into three projects:

| Project | Responsibility |
|---|---|
| `CalcConverter.Core` | Pure logic with no console code. Does the math, applies the rules, parses text into numbers. |
| `CalcConverter.Cli` | Talks to the user. Reads input, calls Core, formats and prints results. |
| `CalcConverter.Tests` | Unit tests for Core and scripted end-to-end tests for the console app. |

Keeping logic out of the console makes it easy to test and easy to reuse in another interface later.
The full reasoning behind each decision is in [docs/DESIGN.md](docs/DESIGN.md).

## Testing

95 xUnit tests run locally with `dotnet test` and automatically on GitHub for every push and pull request.

| Area | Tests | What they cover |
|---|---|---|
| Calculator | 19 | All four operations, divide by zero, overflow, operation routing |
| LengthConverter | 11 | Both directions, a round trip, negative distances |
| TemperatureConverter | 13 | Known reference points, rounding, absolute zero |
| CurrencyConverter | 9 | Conversions to the cent, invalid rates and amounts |
| InputParser | 27 | Valid and invalid numbers and menu choices |
| InputReader | 4 | Re-prompting on bad input, closed input stream |
| App (end to end) | 12 | Full user sessions, including garbage input and errors |

## Assumptions

- **Exchange rate format:** the rate means how many cedis buy one US dollar (for example 15.50), which is how rates are usually quoted in Ghana. USD = GHS / rate.
- **Valid values:** distances and money amounts cannot be negative. Temperatures can be negative but not below absolute zero (-273.15 C / -459.67 F). Zero is allowed everywhere except as an exchange rate or divisor.
- **Number format:** the decimal separator is a dot (`15.5`). Commas are rejected, because `1,500` is ambiguous between fifteen hundred and one and a half.
- **Mile definition:** 1 mile = exactly 1.609344 km (the international mile).
- **Display rounding:** results are shown to 10 decimal places for the calculator, 4 for distances, 2 for temperatures and 2 for money, rounding halves away from zero. Calculations themselves are done at full precision.

## Known limitations

- The calculator handles one operation on two numbers at a time. It does not evaluate expressions like `2 + 3 * 4`, chain results or keep a memory.
- Numbers use C#'s `decimal` type, which supports about 28 significant digits and values up to roughly 7.9 x 10^28. Larger results show a "too large" error.
- Scientific notation (for example `1e5`) and thousands separators are not accepted as input.
- Currency conversion is one way only (GHS to USD) and uses the rate the user types in. There are no live exchange rates.
- When a value is rejected (for example a negative distance), the user returns to the main menu instead of re-entering just that value.
- The interface is English only.

## Development workflow

Work was done on short-lived branches and merged into `main` through pull requests:

- `chore/solution-scaffold`: solution and project structure
- `feat/calculator`: calculator and its tests
- `feat/converters`: length, temperature and currency converters
- `feat/console-ui`: input handling, menus and end-to-end tests
- `docs/readme-design-ci`: documentation and CI

Commit messages follow the [Conventional Commits](https://www.conventionalcommits.org/) style (`feat:`, `test:`, `docs:`, `ci:`, `chore:`).

## Author

Gyimah Ramsey Opoku ([@aimlin9](https://github.com/aimlin9))