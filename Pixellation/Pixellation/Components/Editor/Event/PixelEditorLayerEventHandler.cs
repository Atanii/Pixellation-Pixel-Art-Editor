#nullable enable

namespace Pixellation.Components.Event
{
    /// <summary>
    /// Parameterized event handler for events regarding layers.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PixelEditorLayerEventHandler(object? sender, PixelEditorLayerEventArgs e);
}