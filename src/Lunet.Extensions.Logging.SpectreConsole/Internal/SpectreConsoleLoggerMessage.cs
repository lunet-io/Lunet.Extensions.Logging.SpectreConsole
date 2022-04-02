// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Spectre.Console.Rendering;

namespace Lunet.Extensions.Logging.SpectreConsole;

/// <summary>
/// Logger message used internally supporting Spectre markup.
/// </summary>
/// <param name="Text">An optional message that can be markup.</param>
/// <param name="Renderables">An array of renderable objects to render after the message.</param>
internal record SpectreConsoleLoggerMessage(string? Text, IRenderable[] Renderables)
{
    public override string ToString()
    {
        var console = SpectreConsoleLoggerRenderer.CreateAsciiOffScreenConsole();
        return SpectreConsoleLoggerRenderer.Render(console, this , null);
    }
}