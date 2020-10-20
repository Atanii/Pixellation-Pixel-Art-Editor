namespace Pixellation.Components.Tools
{
    public class ToolEventArgs
    {
        public ToolEventType Type { get; set; } = ToolEventType.NOTHING;
        public object Value { get; set; } = default;
    }
}