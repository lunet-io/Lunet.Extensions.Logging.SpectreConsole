// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Spectre.Console;

namespace Lunet.Extensions.Logging.SpectreConsole;

/// <summary>
/// Off-screen renderer.
/// </summary>
internal class SpectreConsoleLoggerRenderer
{
    /// <summary>
    /// Renders a log Message to an off-screen console.
    /// </summary>
    /// <param name="console">An offscreen console created by <see cref="CreateAsciiOffScreenConsole"/> or <see cref="CreateOffScreenConsole"/>.</param>
    /// <param name="logMessage">The log message to log</param>
    /// <param name="exception">An optional exception.</param>
    /// <returns>The string representation of the log message.</returns>
    public static string Render(IAnsiConsole console, SpectreConsoleLoggerMessage logMessage, Exception? exception = null)
    {
        var (text, renderables) = logMessage;

        if (!string.IsNullOrEmpty(text))
        {
            console.MarkupLine(text!);
        }

        foreach (var renderable in renderables)
        {
            console.Write(renderable);
        }

        if (exception is not null)
        {
            console.WriteLine(Markup.Escape(exception.ToString()));
        }

        // Remove any trailing newline
        var output = console.Profile.Out?.ToString() ?? string.Empty;
        if (output.EndsWith("\r\n"))
        {
            output = output.Substring(0, output.Length - 2);
        }
        else if (output.EndsWith("\n"))
        {
            output = output.Substring(0, output.Length - 1);
        }

        return output;
    }

    /// <summary>
    /// Creates an ascii-only off-screen console.
    /// </summary>
    /// <returns>A console to render to ascii.</returns>
    public static IAnsiConsole CreateAsciiOffScreenConsole()
    {
        var offScreenOutput = new AnsiConsoleStringWriterOutput(80, 80);
        var offScreenConsole = AnsiConsole.Create(new AnsiConsoleSettings()
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            ExclusivityMode = null,
            Interactive = InteractionSupport.No,
            Out = offScreenOutput,
        });

        return offScreenConsole;
    }

    /// <summary>
    /// Creates an off-screen console that replicates the parameters from an existing console.
    /// </summary>
    /// <param name="consoleSettings">The settings of the original console.</param>
    /// <param name="console">The original console to replicate parameters from.</param>
    /// <returns>The offscreen console.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IAnsiConsole CreateOffScreenConsole(AnsiConsoleSettings consoleSettings, IAnsiConsole console)
    {
        var offScreenOutput = new AnsiConsoleStringWriterOutput(console.Profile.Out.Width, console.Profile.Out.Height);
        // Fetch the encoding from the current console
        offScreenOutput.SetEncoding(console.Profile.Encoding);
        
        var offScreenConsole = AnsiConsole.Create(new AnsiConsoleSettings()
        {
            // Fetch what was detected
            Ansi = console.Profile.Capabilities.Ansi ? AnsiSupport.Yes : AnsiSupport.No,
            ColorSystem = GetColorSystemSupport(console.Profile.Capabilities.ColorSystem),
            Enrichment = consoleSettings.Enrichment,
            EnvironmentVariables = consoleSettings.EnvironmentVariables,
            ExclusivityMode = null,
            Interactive = InteractionSupport.No,
            Out = offScreenOutput,
        });
        
        return offScreenConsole;

        static ColorSystemSupport GetColorSystemSupport(ColorSystem colorSystem)
        {
            return colorSystem switch
            {
                ColorSystem.NoColors => ColorSystemSupport.NoColors,
                ColorSystem.Legacy => ColorSystemSupport.Legacy,
                ColorSystem.Standard => ColorSystemSupport.Standard,
                ColorSystem.EightBit => ColorSystemSupport.EightBit,
                ColorSystem.TrueColor => ColorSystemSupport.TrueColor,
                _ => throw new ArgumentOutOfRangeException(nameof(colorSystem), colorSystem, null)
            };
        }
    }

    /// <summary>
    /// Implement an <see cref="IAnsiConsoleOutput"/> to a StringBuilder.
    /// </summary>
    private class AnsiConsoleStringWriterOutput : IAnsiConsoleOutput
    {
        private readonly StringBuilder _builder;

        public AnsiConsoleStringWriterOutput(int width, int height)
        {
            _builder = new StringBuilder();
            Writer = new StringWriter(_builder);
            IsTerminal = false;
            Width = width <= 0 || width == int.MaxValue ? 80 : width;
            Height = height <= 0 || height == int.MaxValue ? 80 : height;
        }

        [ExcludeFromCodeCoverage]
        public void SetEncoding(Encoding encoding)
        {
        }

        [ExcludeFromCodeCoverage]
        public TextWriter Writer { get; }

        [ExcludeFromCodeCoverage]
        public bool IsTerminal { get; }

        [ExcludeFromCodeCoverage]
        public int Width { get; }

        [ExcludeFromCodeCoverage]
        public int Height { get; }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}