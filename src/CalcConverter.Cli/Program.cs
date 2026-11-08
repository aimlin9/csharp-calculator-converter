using CalcConverter.Cli;

// Entry point.
// Kept deliberately tiny: all behaviour lives in App, which takes its input and
// output as parameters. That way the tests can drive the exact same code with
// scripted input instead of a real keyboard.
var app = new App(Console.In, Console.Out);

return app.Run();