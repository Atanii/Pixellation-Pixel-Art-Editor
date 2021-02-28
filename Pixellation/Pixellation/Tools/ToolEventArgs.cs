namespace Pixellation.Tools
{
    /// <summary>
    /// Args for event regarding tools.
    /// </summary>
    public class ToolEventArgs
    {
        /// <summary>
        /// Type of event.
        /// </summary>
        public ToolEventType Type { get; set; } = ToolEventType.NOTHING;

        /// <summary>
        /// New or picked color.
        /// </summary>
        public System.Drawing.Color Value { get; set; } = default;
    }
}