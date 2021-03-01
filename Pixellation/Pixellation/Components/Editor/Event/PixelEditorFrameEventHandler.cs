﻿#nullable enable

namespace Pixellation.Components.Event
{
    /// <summary>
    /// Parameterized event handler for events regarding frames.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PixelEditorFrameEventHandler(object? sender, PixelEditorFrameEventArgs e);
}