namespace Pixellation.Tools
{
    public class ToolEventArgs
    {
        public ToolEventType Type { get; set; } = ToolEventType.NOTHING;
        public object Value { get; set; } = default;
    }
}