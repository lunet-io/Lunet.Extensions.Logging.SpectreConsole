// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Spectre.Console;

namespace Lunet.Extensions.Logging.SpectreConsole.Tests
{
    public class Tests
    {
        private static AnsiConsoleSettings SimpleAnsiConsoleSettings => new AnsiConsoleSettings()
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Out = new CustomConsole()
        };

        [Test]
        public void TestSimple()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => { configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                {
                    ConsoleSettings = SimpleAnsiConsoleSettings
                }); });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
                logger.LogError("Test2\nTest2.1\nTest2.2");
                logger.LogInformation("Test3");
                logger.LogInformationMarkup("[red]Test4[/]");
            }, @"info: hello
      Test1
fail: hello
      Test2
      Test2.1
      Test2.2
info: hello
      Test3
info: hello
      Test4
");
        }

        [Test]
        public void TestSimpleFixedIndent()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        UseFixedIndent = true,
                        FixedIndent = 10
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
                logger.LogError("Test2\nTest2.1\nTest2.2");
                logger.LogInformation("Test3");
                logger.LogInformationMarkup("[red]Test4[/]");
            }, @"info: hello
          Test1
fail: hello
          Test2
          Test2.1
          Test2.2
info: hello
          Test3
info: hello
          Test4
");
        }

        [Test]
        public void TestSimpleCheckConfigureConsole()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConfigureConsole = console =>
                        {
                            Console.WriteLine("Console configured");
                        },
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
            }, @"Console configured
info: hello
      Test1
");
        }

        [Test]
        public void TestSimpleLogLevel()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
                logger.LogDebug("Test2");
            }, @"info: hello
      Test1
");
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        LogLevel = LogLevel.Debug
                    });
                    configure.SetMinimumLevel(LogLevel.Debug);
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
                logger.LogDebug("Test2");
            }, @"info: hello
      Test1
dbug: hello
      Test2
");
        }

        [Test]
        public void TestSimpleNoIndent()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => { configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                {
                    ConsoleSettings = SimpleAnsiConsoleSettings,
                    IndentAfterNewLine = false,
                }); });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
                logger.LogError("Test2\nTest2.1\nTest2.2");
                logger.LogInformation("Test3");
                logger.LogInformationMarkup("[red]Test4[/]");
            }, @"info: hello
Test1
fail: hello
Test2
Test2.1
Test2.2
info: hello
Test3
info: hello
Test4
");
        }

        [Test]
        public void TestSimpleNoIndentNoNewLine()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        IncludeNewLineBeforeMessage = false,
                        IndentAfterNewLine = false,
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
                logger.LogError("Test2\nTest2.1\nTest2.2");
                logger.LogInformation("Test3");
                logger.LogInformationMarkup("[red]Test4[/]");
            }, @"info: hello Test1
fail: hello Test2
Test2.1
Test2.2
info: hello Test3
info: hello Test4
");
        }

        [Test]
        public void TestSimpleNoIndentNoNewLineSingleLine()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        SingleLine = true,
                        IncludeNewLineBeforeMessage = false,
                        IndentAfterNewLine = false,
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation("Test1");
                logger.LogError("Test2\nTest2.1\nTest2.2");
                logger.LogInformation("Test3");
                logger.LogInformationMarkup("[red]Test4[/]");
            }, @"info: hello Test1
fail: hello Test2 Test2.1 Test2.2
info: hello Test3
info: hello Test4
");
        }


        [Test]
        public void TestSimpleWithEventId()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => { configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                {
                    ConsoleSettings = SimpleAnsiConsoleSettings,
                }); });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation(1, "Test1");
                logger.LogError(2, "Test2");
            }, @"info: hello[1] 
      Test1
fail: hello[2] 
      Test2
");
        }

        [Test]
        public void TestSimpleWithEventIdWithCustomFormat()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure => { configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                {
                    ConsoleSettings = SimpleAnsiConsoleSettings,
                    EventIdFormat = "(0000)"
                }); });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation(1, "Test1");
                logger.LogError(2, "Test2");
            }, @"info: hello[(0001)] 
      Test1
fail: hello[(0002)] 
      Test2
");
        }

        [Test]
        public void TestSimpleWithoutCategory()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        IncludeCategory = false,
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation(1, "Test1");
                logger.LogError(2, "Test2");
            }, @"info: [1] 
      Test1
fail: [2] 
      Test2
");
        }

        [Test]
        public void TestSimpleWithoutCategoryAndEventId()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        IncludeEventId = false,
                        IncludeCategory = false,
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation(1, "Test1");
                logger.LogError(2, "Test2");
            }, @"info: 
      Test1
fail: 
      Test2
");
        }

        [Test]
        public void TestSimpleWithoutCategoryAndEventIdAndLogLevel()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        IncludeLogLevel = false,
                        IncludeEventId = false,
                        IncludeCategory = false,
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation(1, "Test1");
                logger.LogError(2, "Test2");
            }, @"
Test1

Test2
");
        }

        [Test]
        public void TestSimpleWithTimeStamp()
        {
            Run(() =>
            {
                var baseDate = new DateTime(2022, 3, 22, 7, 55, 00, 125);
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        IncludeTimestamp = true,
                        GetLogTimeStamp = () =>
                        {
                            var date = baseDate;
                            baseDate = baseDate.AddHours(1);
                            return date;
                        }
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation(1, "Test1");
                logger.LogInformation(2, "Test2");
            }, @"2022/03/22 07:55:00.125 info: hello[1] 
                              Test1
2022/03/22 08:55:00.125 info: hello[2] 
                              Test2
");
        }

        [Test]
        public void TestWithRenderableRule()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                    });
                });
                var logger = factory.CreateLogger("hello");

                var bar = new Rule("yes");

                logger.LogInformationMarkup(1, "Test1", bar);
            }, @"info: hello[1] 
      Test1
      ───────────────────────────────────── yes ──────────────────────────────────────
");
        }

        [Test]
        public void TestWithRenderableTable()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                    });
                });
                var logger = factory.CreateLogger("hello");

                var table = new Table();
                table.AddColumn("First");
                table.AddColumn("Second");
                table.AddRow("1", "2");
                table.AddRow("3", "4");

                logger.LogInformationMarkup(1, "Test1", table);
            }, @"info: hello[1] 
      Test1
      ┌───────┬────────┐
      │ First │ Second │
      ├───────┼────────┤
      │ 1     │ 2      │
      │ 3     │ 4      │
      └───────┴────────┘
");
        }

        [Test]
        public void TestSimpleWithForcedColors()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = new AnsiConsoleSettings()
                        {
                            Ansi = AnsiSupport.Yes,
                            ColorSystem = ColorSystemSupport.TrueColor
                        }
                    });
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformation(1, "Test1");
                logger.LogError(2, "Test2");
            }, "\u001b[38;5;2;48;5;0minfo\u001b[0m: hello\u001b[38;5;8;48;5;0m[1]\u001b[0m \n      Test1\n\u001b[38;5;0;48;5;1mfail\u001b[0m: hello\u001b[38;5;8;48;5;0m[2]\u001b[0m \n      Test2\n");
        }

        [Test]
        public void TestAllLogExtensions()
        {
            Run(() =>
            {
                var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSpectreConsole(new SpectreConsoleLoggerOptions()
                    {
                        ConsoleSettings = SimpleAnsiConsoleSettings,
                        LogException = true,
                    });
                    configure.SetMinimumLevel(LogLevel.Trace);
                });
                var logger = factory.CreateLogger("hello");

                logger.LogTraceMarkup(1, new Exception(), "Test1");
                logger.LogTraceMarkup(2, "Test2");
                logger.LogTraceMarkup(new Exception(), "Test3");
                logger.LogTraceMarkup("Test4");

                logger.LogDebugMarkup(1, new Exception(), "Test1");
                logger.LogDebugMarkup(2, "Test2");
                logger.LogDebugMarkup(new Exception(), "Test3");
                logger.LogDebugMarkup("Test4");
                
                logger.LogWarningMarkup(1, new Exception(), "Test1");
                logger.LogWarningMarkup(2, "Test2");
                logger.LogWarningMarkup(new Exception(), "Test3");
                logger.LogWarningMarkup("Test4");

                logger.LogInformationMarkup(1, new Exception(), "Test1");
                logger.LogInformationMarkup(2, "Test2");
                logger.LogInformationMarkup(new Exception(), "Test3");
                logger.LogInformationMarkup("Test4");

                logger.LogErrorMarkup(1, new Exception(), "Test1");
                logger.LogErrorMarkup(2, "Test2");
                logger.LogErrorMarkup(new Exception(), "Test3");
                logger.LogErrorMarkup("Test4");

                logger.LogCriticalMarkup(1, new Exception(), "Test1");
                logger.LogCriticalMarkup(2, "Test2");
                logger.LogCriticalMarkup(new Exception(), "Test3");
                logger.LogCriticalMarkup("Test4");
            }, @"trce: hello[1] 
      Test1
      System.Exception: Exception of type 'System.Exception' was thrown.
trce: hello[2] 
      Test2
trce: hello
      Test3
      System.Exception: Exception of type 'System.Exception' was thrown.
trce: hello
      Test4
dbug: hello[1] 
      Test1
      System.Exception: Exception of type 'System.Exception' was thrown.
dbug: hello[2] 
      Test2
dbug: hello
      Test3
      System.Exception: Exception of type 'System.Exception' was thrown.
dbug: hello
      Test4
warn: hello[1] 
      Test1
      System.Exception: Exception of type 'System.Exception' was thrown.
warn: hello[2] 
      Test2
warn: hello
      Test3
      System.Exception: Exception of type 'System.Exception' was thrown.
warn: hello
      Test4
info: hello[1] 
      Test1
      System.Exception: Exception of type 'System.Exception' was thrown.
info: hello[2] 
      Test2
info: hello
      Test3
      System.Exception: Exception of type 'System.Exception' was thrown.
info: hello
      Test4
fail: hello[1] 
      Test1
      System.Exception: Exception of type 'System.Exception' was thrown.
fail: hello[2] 
      Test2
fail: hello
      Test3
      System.Exception: Exception of type 'System.Exception' was thrown.
fail: hello
      Test4
crit: hello[1] 
      Test1
      System.Exception: Exception of type 'System.Exception' was thrown.
crit: hello[2] 
      Test2
crit: hello
      Test3
      System.Exception: Exception of type 'System.Exception' was thrown.
crit: hello
      Test4
");
        }

        [Test]
        public void TestLoggingMarkupWithoutSpectre()
        {
            Run(() =>
            {
                using var factory = LoggerFactory.Create(configure =>
                {
                    configure.AddSimpleConsole();
                });
                var logger = factory.CreateLogger("hello");
                logger.LogInformationMarkup(1, "[red]Test1[/]");
                logger.LogError(2, "Test2");
                var bar = new Rule("yes");
                logger.LogInformationMarkup(3, "Test3", bar);
            }, @"info: hello[1]
      Test1
fail: hello[2]
      Test2
info: hello[3]
      Test3
      ───────────────────────────────────── yes ──────────────────────────────────────
");
        }

        private static void Run(Action action, string expected)
        {
            var writer = new StringWriter();
            var stdout = Console.Out;
            Console.SetOut(writer);
            try
            {
                action();
            }
            finally
            {
                Console.SetOut(stdout);
            }

            var result = writer.ToString();
            TextEqual(expected, result);
        }
        
        private static void TextEqual(string expected, string result)
        {
            expected = expected.Replace("\r\n", "\n");
            result = result.Replace("\r\n", "\n");
            if (expected != result)
            {
                Console.WriteLine("Result");
                Console.WriteLine("**************************************************");
                Console.WriteLine(result);

                Console.WriteLine("Expected");
                Console.WriteLine("**************************************************");
                Console.WriteLine(expected);
            }

            Assert.AreEqual(expected, result);
        }


        private class CustomConsole : IAnsiConsoleOutput
        {
            public CustomConsole()
            {
                Writer = Console.Out;
                Width = 80;
                Height = 80;
                IsTerminal = false;
            }

            public void SetEncoding(Encoding encoding)
            {
            }

            public TextWriter Writer { get; }
            public bool IsTerminal { get; }
            public int Width { get; }
            public int Height { get; }
        }
    }
}