// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Lunet.Extensions.Logging.SpectreConsole;

/// <summary>
/// An <see cref="ILoggerProvider"/> for Spectre.Console.
/// </summary>
public class SpectreConsoleLoggerProvider : ILoggerProvider
{
    private readonly SpectreConsoleLoggerOptions _config;
    private readonly ConcurrentDictionary<string, SpectreConsoleLogger> _loggers = new();
    private readonly IAnsiConsole _console;

    /// <summary>
    /// Creates an instance of this class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    public SpectreConsoleLoggerProvider(SpectreConsoleLoggerOptions config)
    {
        _config = config;
        _console = AnsiConsole.Create(config.ConsoleSettings);
        config.ConfigureConsole?.Invoke(_console);
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new SpectreConsoleLogger(name, _console, _config));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _loggers.Clear();
    }
}