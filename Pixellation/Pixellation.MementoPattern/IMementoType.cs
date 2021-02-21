namespace Pixellation.Utils.MementoPattern
{
    /// <summary>
    /// Base interface for an "enum-like" type listing different possible events that the memento-creation preceeds.
    /// </summary>
    public interface IMementoType {
        /// <summary>
        /// Default, "empty" value.
        /// </summary>
        public const int NONE = 0;
    }
}