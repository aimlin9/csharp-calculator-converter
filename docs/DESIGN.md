# Design and Engineering Decisions

This document explains how the application is structured and why each key decision was made.

## 1. Architecture

```text
┌──────────────────────────┐
│     CalcConverter.Cli    │   Program.cs  ->  App  ->  InputReader
│  (talks to the user)     │
└────────────┬─────────────┘
             │ calls
┌────────────▼─────────────┐
│    CalcConverter.Core    │   Calculator, LengthConverter,
│  (logic and rules only)  │   TemperatureConverter, CurrencyConverter,
└────────────▲─────────────┘   InputParser
             │ tested by
┌────────────┴─────────────┐
│   CalcConverter.Tests    │   Unit tests (Core) and end-to-end tests (Cli)
└──────────────────────────┘
```

Dependencies point one way only: the console app depends on Core, and Core depends on nothing. Core has no idea a console exists, so the same logic could sit behind a web API or desktop app without changes.

### Responsibilities

| Component | Responsibility |
|---|---|
| `Calculator` | The four arithmetic operations, plus a `Calculate` method that routes by `Operation`. |
| `LengthConverter`, `TemperatureConverter`, `CurrencyConverter` | One formula per direction, plus validation of physically or logically impossible values. |
| `InputParser` | Turns user text into numbers and menu choices without throwing. |
| `InputReader` | Prompts the user and keeps asking until input is valid. |
| `App` | Menu loop, screens, error reporting and result formatting. |
| `Program.cs` | Two-line entry point that starts `App` with the real console. |

## 2. Key decisions

### `decimal` instead of `double`
`double` stores numbers in binary, so `0.1 + 0.2` gives `0.30000000000000004`. `decimal` stores base-10 values exactly and gives `0.3`, which is what someone using a calculator expects. It also suits money. The conversion constants (1.609344, 9/5, 32) are exact in `decimal` too, so results like 37 C = 98.6 F come out exactly.

The trade-off is range: `decimal` tops out around 7.9 x 10^28. Results beyond that throw an `OverflowException`, which the app catches and explains to the user.

### Validation lives in Core
Rules such as "a distance cannot be negative" or "nothing is colder than absolute zero" are part of the domain, not the interface. Putting them in Core means every interface gets the same rules for free, and the rules are unit tested directly. Core reports violations with an `ArgumentException` whose message is written for end users, so the console shows it as-is.

### Two strategies for bad input
- **Typing mistakes** (letters, empty input, `1,5`) are expected, so `InputParser` uses the Try pattern (`TryParseNumber` returns `false`). No exceptions, just a hint and another prompt.
- **Rule violations** (divide by zero, negative distance) are exceptions raised by Core. `App` catches them in one place per screen (`RunScreen`), shows the message and returns to the menu.

This keeps exceptions for exceptional cases and keeps the error handling in a single, predictable location.

### Commas are rejected, and parsing uses InvariantCulture
`1,500` could mean fifteen hundred or one and a half depending on the country. Guessing wrong would produce a silently wrong answer, which is worse than asking the user to retype. Parsing with `CultureInfo.InvariantCulture` also means the app reads input the same way on every machine, regardless of regional settings.

### Rounding only at the edge
Core always returns full-precision results. `App` rounds only when displaying them, using `MidpointRounding.AwayFromZero` because that matches what most people expect (2.5 rounds to 3). .NET's default is banker's rounding (2.5 rounds to 2).

### Console access is injected
`App` and `InputReader` take a `TextReader` and `TextWriter` instead of calling `Console` directly. The real app passes `Console.In` and `Console.Out`; tests pass in-memory text. This is what makes the end-to-end tests possible: a whole user session is scripted line by line and the output is checked.

### Closed input is handled explicitly
If the input stream closes (Ctrl+Z on Windows, or piped input running out), `Console.ReadLine()` returns `null` forever. Without handling, the "ask again" loop would spin endlessly. `InputReader` turns this into an `InputEndedException`, and `App` catches it and exits cleanly with code 0.

### Explicit menu mapping
Menu numbers are mapped to operations with a `switch`, not by casting the number to the enum. Reordering the `Operation` enum can then never silently change what a menu option does.

### The exchange rate direction
The rate is entered as "cedis per one US dollar" because that is how banks and forex bureaus in Ghana quote it. The prompt spells this out with an example (15.50), so users are unlikely to enter it the wrong way round.

## 3. Testing strategy

Two layers of tests, 95 in total:

1. **Unit tests for Core** cover each operation and conversion using well-known reference points (0 C = 32 F, 1 mile = 1.609344 km), plus boundaries (absolute zero is accepted, just below it is rejected) and invalid input.
2. **End-to-end tests for the console app** feed scripted input into `App` and check the printed output. They prove the app shows correct results, reports every kind of error, survives garbage input at every prompt, and exits cleanly when input ends.

Data-driven tests (`[Theory]` with `TheoryData`) keep cases readable and make it cheap to add more. A round-trip test (km to miles and back) catches mismatched formulas that single-direction tests could miss.

GitHub Actions runs the full suite on every push to `main` and every pull request.

## 4. Things I would change with more time

- **Consistency between classes:** `Calculator` is an instance class while the converters are static. Both are stateless, so this is a small inconsistency rather than a deliberate pattern. Making them uniform, possibly behind interfaces, would also allow mocking Core when testing the console layer.
- **Re-prompt instead of returning to the menu** when Core rejects a value, so the user only re-enters the one bad number.
- **Expression evaluation** (`2 + 3 * 4`) and a running result for chained calculations.
- **USD to GHS** and other currency pairs, with an optional live rate lookup.