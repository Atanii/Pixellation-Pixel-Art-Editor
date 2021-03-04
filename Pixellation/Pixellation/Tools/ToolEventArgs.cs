using System.Drawing;

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
        public Color Value { get; set; } = default;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">Type of event that happened.</param>
        /// <param name="c">Color accompaining the event.</param>
        public ToolEventArgs(ToolEventType type = ToolEventType.DRAW, Color c = default)
        {
            Type = type;
            Value = c;
        }
    }
}