// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Lunet.Extensions.Logging.SpectreConsole;

/// <summary>
/// Options for <see cref="SpectreConsoleLoggerProvider"/>.
/// </summary>
public class SpectreConsoleLoggerOptions
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SpectreConsoleLoggerOptions()
    {
        LogLevel = LogLevel.Information;
        ConsoleSettings = new AnsiConsoleSettings()
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect
        };
        IncludeTimestamp = false;
        IncludeLogLevel = true;
        IncludeCategory = true;
        CultureInfo = CultureInfo.InvariantCulture;
        EventIdFormat = "###0";
        TimestampFormat = "yyyy/MM/dd HH:mm:ss.fff";
        IncludeEventId = true;
        IncludeNewLineBeforeMessage = true;
        IndentAfterNewLine = true;
        LogException = false;
        SingleLine = false;
        Formatter = SpectreConsoleLoggerFormatter.Default;
        TimestampFormatter = SpectreConsoleLoggerFormatter.DefaultTimestampFormatter;
        EventIdFormatter = SpectreConsoleLoggerFormatter.DefaultEventIdFormatter;
        LogLevelFormatter = SpectreConsoleLoggerFormatter.DefaultLogLevelFormatter;
        CategoryFormatter = SpectreConsoleLoggerFormatter.DefaultCategoryFormatter;
        FixedIndent = 4;
        GetLogTimeStamp = () => DateTime.Now;
    }

    /// <summary>
    /// Gets or sets the minimum log level to log. Default is Information.
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// Gets or sets a callback to allow to configure the console once created from <see cref="ConsoleSettings"/>.
    /// </summary>
    public Action<IAnsiConsole>? ConfigureConsole { get; set; }

    /// <summary>
    /// Gets or sets the settings of the console.
    /// </summary>
    public AnsiConsoleSettings ConsoleSettings { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log should include a timestamp. Default is <c>false</c>.
    /// </summary>
    public bool IncludeTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the formatting string for the timestamp. Default is "yyyy/MM/dd HH:mm:ss.fff".
    /// </summary>
    public string TimestampFormat { get; set; }

    /// <summary>
    /// Gets or sets the formatting string for the <see cref="EventId"/>. Default is "####".
    /// </summary>
    public string EventIdFormat { get; set; }

    /// <summary>
    /// Gets or sets the culture used for formatting. Default is invariant.
    /// </summary>
    public CultureInfo CultureInfo { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log should include the log level. Default is <c>true</c>.
    /// </summary>
    public bool IncludeLogLevel { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log should include the log category. Default is <c>true</c>.
    /// </summary>
    public bool IncludeCategory { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log should include the log event id. Default is <c>true</c>.
    /// </summary>
    public bool IncludeEventId { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log should include a new line right before the message. Default is <c>true</c>.
    /// </summary>
    public bool IncludeNewLineBeforeMessage { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log should indent on new lines. Default is <c>true</c>.
    /// </summary>
    public bool IndentAfterNewLine { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log should use a fix indent on new lines instead of the automatic indent. Default is <c>false</c>.
    /// </summary>
    public bool UseFixedIndent { get; set; }

    /// <summary>
    /// Gets or sets the fixed indent level if <see cref="UseFixedIndent"/> is <c>true</c>.
    /// </summary>
    public int FixedIndent { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating whether to log exceptions to the output. Default is <c>false</c>.
    /// </summary>
    public bool LogException { get; set; }

    /// <summary>
    /// Gets or sets a boolean indicating if the log message should be emitted on a single line. Default is <c>false</c>. 
    /// </summary>
    /// <remarks>
    /// Note that if <see cref="IncludeNewLineBeforeMessage"/>, a new line will still be emitted before the log message.
    /// </remarks>
    public bool SingleLine { get; set; }

    /// <summary>
    /// Gets or sets the callback to get the log timestamp. Default is Datetime.Now.
    /// </summary>
    public Func<DateTime> GetLogTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the formatter used to format the datetime, log level, event id and category.
    /// </summary>
    public SpectreConsoleLoggerFormatterDelegate Formatter { get; set; }

    /// <summary>
    /// Gets or sets the formatter for the timestamp of a log.
    /// </summary>
    public Action<SpectreConsoleLoggerOptions, StringBuilder, DateTime> TimestampFormatter { get; set; }

    /// <summary>
    /// Gets or sets the formatter for the log level.
    /// </summary>
    public Action<SpectreConsoleLoggerOptions, StringBuilder, LogLevel> LogLevelFormatter { get; set; }

    /// <summary>
    /// Gets or sets the formatter for the log event id.
    /// </summary>
    public Action<SpectreConsoleLoggerOptions, StringBuilder, EventId> EventIdFormatter { get; set; }

    /// <summary>
    /// Gets or sets the formatter for the log category.
    /// </summary>
    public Action<SpectreConsoleLoggerOptions, StringBuilder, string> CategoryFormatter { get; set; }
}