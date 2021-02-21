#nullable enable

namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Eventhandler for events regarding <see cref="Caretaker{_MementoType}"/>.
    /// </summary>
    /// <param name="sender">Object invoking this event.</param>
    /// <param name="e">Arguments describing the current state of saved operations.</param>
    public delegate void CaretakerEventHandler(object? sender, CaretakerEventArgs e);
}