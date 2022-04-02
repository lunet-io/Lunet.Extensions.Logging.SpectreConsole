// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Text;
using Microsoft.Extensions.Logging;

namespace Lunet.Extensions.Logging.SpectreConsole;


/// <summary>
/// Delegate to format a log message with Spectre markup.
/// </summary>
/// <param name="options">The options.</param>
/// <param name="builder">A string builder to output the message to.</param>
/// <param name="category">The category of the log.</param>
/// <param name="logLevel">The log level.</param>
/// <param name="eventId">The event id.</param>
/// <returns>The character column index as a positive integer to specify where new lines will be indented if <see cref="SpectreConsoleLoggerOptions.IndentAfterNewLine"/> is **true**.</returns>
public delegate int SpectreConsoleLoggerFormatterDelegate(SpectreConsoleLoggerOptions options, StringBuilder builder, string category, LogLevel logLevel, EventId eventId);