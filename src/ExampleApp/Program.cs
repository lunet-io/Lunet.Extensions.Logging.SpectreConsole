using Lunet.Extensions.Logging.SpectreConsole;
using Microsoft.Extensions.Logging;
using Spectre.Console;

// Example1: default layout (Similar to SimpleConsole)
using (var factory = LoggerFactory.Create(configure => configure.AddSpectreConsole()))
{
    var logger = factory.CreateLogger("SampleCategory");
    logger.LogInformationMarkup("Hello with [red]SpectreConsole[/]");
    logger.LogWarning("Hello without markup");
}

// Example2: Don't add a new line, include timestamp
using (var factory = LoggerFactory.Create(configure =>
           {
               configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
               {
                   IncludeNewLineBeforeMessage = false,
                   IncludeTimestamp = true,
               });
           }
       ))
{
    var logger = factory.CreateLogger("SampleCategory");
    logger.LogInformationMarkup(new EventId(1), "Hello from [red]SpectreConsole[/]");
    logger.LogWarning(new EventId(2), "Hello without markup");
}

// Example3: Don't add a new line, include timestamp, log a table
using (var factory = LoggerFactory.Create(configure =>
           {
               configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
               {
                   IncludeNewLineBeforeMessage = false,
                   IncludeTimestamp = true,
               });
           }
       ))
{

    var table = new Table();
    table.AddColumn("Name");
    table.AddColumn("Spectre?");
    table.AddRow("Microsoft.Extensions.Logging.Console", "⛔");
    table.AddRow("Lunet.Extensions.Logging.SpectreConsole", "✅");
    
    var logger = factory.CreateLogger("SampleCategory");
    logger.LogInformationMarkup(new EventId(1), "Hello from [red]SpectreConsole[/] with a table:", table);
    logger.LogWarning(new EventId(2), "Hello without markup");
}
