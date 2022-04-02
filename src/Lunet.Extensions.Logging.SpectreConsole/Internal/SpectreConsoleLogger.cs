// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Wcwidth;

namespace Lunet.Extensions.Logging.SpectreConsole;

/// <summary>
/// Internal implementation of ILogger with Spectre.Console.
/// </summary>
internal class SpectreConsoleLogger : ILogger
{
    [ThreadStatic]
    internal static bool EnableMarkup;

    [ThreadStatic]
    private static StringBuilder? _stringBuilderTls;
    private static StringBuilder StringBuilderTls => _stringBuilderTls ??= new StringBuilder();

    private readonly string _name;
    private readonly SpectreConsoleLoggerOptions _options;
    private readonly IAnsiConsole _console;

    public SpectreConsoleLogger(string name, IAnsiConsole console, SpectreConsoleLoggerOptions options)
    {
        _name = name;
        _console = console;
        _options = options;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var builder = StringBuilderTls;
        try
        {
            int indexAfterLogLevel = _options.Formatter(_options, builder, _name, logLevel, eventId);
            
            // Calculate indent
            int indent = 0;
            if ((_options.IncludeNewLineBeforeMessage || _options.IndentAfterNewLine))
            {
                if (_options.UseFixedIndent)
                {
                    // Fixed indentation
                    indent = _options.FixedIndent;
                }
                else
                {
                    var prefix = Markup.Remove(builder.ToString(0, indexAfterLogLevel));

                    // Calculate the indentation level up to the category/eventid
                    for (int i = 0; i < prefix.Length; i++)
                    {
                        var c = prefix[i];
                        int uni = c;
                        if (char.IsHighSurrogate(c) && i + 1 < indexAfterLogLevel && char.IsLowSurrogate(prefix[i + 1]))
                        {
                            i++;
                            uni = char.ConvertToUtf32(c, prefix[i]);
                        }

                        indent += UnicodeCalculator.GetWidth(uni);
                    }
                }
            }

            string formattedMessage;
            bool rawFormattedMessage = false;
            StringBuilder builderForMessage = builder;
            bool enableMarkup = EnableMarkup;

            if (state is SpectreConsoleLoggerMessage messageAndRenderable)
            {
                if (messageAndRenderable.Renderables.Length == 0)
                {
                    formattedMessage = messageAndRenderable.Text ?? string.Empty;

                    if (_options.LogException && exception is not null)
                    {
                        formattedMessage += $"\n{exception}";
                    }
                }
                else
                {
                    var offscreen = SpectreConsoleLoggerRenderer.CreateOffScreenConsole(_options.ConsoleSettings, _console);
                    formattedMessage = SpectreConsoleLoggerRenderer.Render(offscreen, messageAndRenderable, _options.LogException ? exception : null);
                    builderForMessage = new StringBuilder();
                    rawFormattedMessage = true;
                }
            }
            else
            {
                formattedMessage = formatter(state, exception);

                if (_options.LogException && exception is not null)
                {
                    formattedMessage += $"\n{exception}";
                }
            }

            if (!string.IsNullOrEmpty(formattedMessage))
            {
                var message = rawFormattedMessage || enableMarkup ? formattedMessage : Markup.Escape(formattedMessage);

                if ((_options.IncludeNewLineBeforeMessage || _options.IndentAfterNewLine))
                {
                    AppendMessage(builderForMessage, message, indent, _options.IncludeNewLineBeforeMessage, _options.SingleLine, _options.IndentAfterNewLine);
                }
                else
                {
                    if (builderForMessage.Length > 0 && !char.IsWhiteSpace(builderForMessage[builderForMessage.Length - 1]))
                    {
                        builderForMessage.Append(' ');
                    }

                    if (_options.SingleLine)
                    {
                        AppendMessage(builderForMessage, message, 0, _options.IncludeNewLineBeforeMessage, true, false);
                    }
                    else
                    {
                        builderForMessage.Append(message);
                    }
                }
            }

            if (rawFormattedMessage)
            {
                _console.Markup(builder.ToString());
                var writer = _options.ConsoleSettings.Out?.Writer ?? System.Console.Out;
                var message = builderForMessage.ToString();
                if (message.EndsWith("\n"))
                {
                    writer.Write(message);
                }
                else
                {
                    writer.WriteLine(message);
                }
            }
            else
            {
                _console.MarkupLine(builder.ToString());
            }

            // Flush log
            _console.Profile.Out.Writer.Flush();
        }
        finally
        {
            builder.Length = 0;
        }
    }

    private static void AppendMessage(StringBuilder builder, string message, int indent, bool hasNewLine, bool singleLine, bool indentAfterNewLine)
    {
        if (!string.IsNullOrEmpty(message))
        {
            int currentIndex = 0;

            bool isFirstLine = true;
            bool needsNewLine = false;
            while (currentIndex < message.Length)
            {
                needsNewLine = false;
                if (!isFirstLine || hasNewLine)
                {
                    if (!singleLine)
                    {
                        if (!isFirstLine && !hasNewLine)
                        {
                            builder.AppendLine();
                        }

                        if (indentAfterNewLine)
                        {
                            builder.Append(' ', indent);
                        }
                    }
                    else
                    {
                        builder.Append(' ', singleLine ? 1 : indent);
                    }
                }

                var nextIndex = message.IndexOf('\n', currentIndex);
                if (nextIndex >= 0)
                {
                    var nextCurrentIndex = nextIndex + 1;
                    do
                    {
                        nextIndex--;
                    } while (nextIndex >= currentIndex && message[nextIndex] == '\r');

                    var length = nextIndex - currentIndex + 1;
                    if (length > 0)
                    {
                        builder.Append(message, currentIndex, length);
                    }
                    currentIndex = nextCurrentIndex;
                    needsNewLine = true;
                    hasNewLine = false;
                }
                else
                {
                    builder.Append(message, currentIndex, message.Length - currentIndex);
                    break;
                }

                isFirstLine = false;
            }

            if (!singleLine && needsNewLine)
            {
                builder.AppendLine();
            }
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _options.LogLevel;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        // Not supported
        return NullDisposer.Instance;
    }

    private class NullDisposer : IDisposable
    {
        public static readonly NullDisposer Instance = new NullDisposer();
        public void Dispose()
        {
        }
    }
}