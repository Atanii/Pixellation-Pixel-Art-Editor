using Pixellation.Components.Event;
using Pixellation.MementoPattern;
using Pixellation.Utils;

namespace Pixellation.Components.Editor.Memento
{
    /// <summary>
    /// Class representing a saved state of a frame.
    /// </summary>
    public class FrameMemento : BaseMemento<FrameMemento, IPixelEditorEventType>
    {
        /// <summary>
        /// Index of saved frame.
        /// </summary>
        public int FrameIndex { get; set; }

        /// <summary>
        /// Copy of frame to save.
        /// </summary>
        public DrawingFrame Frame { get; set; }

        /// <summary>
        /// Indicates if the state is restored.
        /// </summary>
        private bool _restoredFlag = false;

        /// <summary>
        /// Inits a saved state for a frame.
        /// </summary>
        /// <param name="originatorHandler">Helper class for saving the state.</param>
        /// <param name="typeValue">Value of <see cref="IPixelEditorEventType"/>.</param>
        /// <param name="frameIndex"><see cref="FrameIndex"/></param>
        /// <param name="original">Frame to save.</param>
        public FrameMemento(IOriginatorHandler<FrameMemento, IPixelEditorEventType> originatorHandler, int typeValue, int frameIndex, DrawingFrame original)
            : base(typeValue, originatorHandler)
        {
            FrameIndex = frameIndex;
            Frame = original.Clone(true);
        }

        /// <summary>
        /// Restore saved state.
        /// </summary>
        /// <returns>Memento for redo (or undo in case of redo).</returns>
        public override IMemento<IPixelEditorEventType> Restore()
        {
            _restoredFlag = true;
            return _originatorHandler.HandleRestore(this);
        }

        /// <summary>
        /// Disposes of the corresponding <see cref="Caretaker{_MementoType}"/> instance if this memento was not restored before being removed.
        /// </summary>
        ~FrameMemento()
        {
            if (!_restoredFlag)
            {
                PixellationCaretakerManager.GetInstance().RemoveCaretaker(Frame.Id);
            }
        }
    }
}