using Pixellation.MementoPattern;
using Pixellation.Utils;
using System;
using System.Diagnostics;

namespace Pixellation.Components.Editor.Memento
{
    public class FrameMemento : BaseMemento<FrameMemento, IPixelEditorEventType>, IDisposable
    {
        public int FrameIndex { get; set; }

        public DrawingFrame Frame { get; set; }

        public FrameMemento(IOriginatorHandler<FrameMemento, IPixelEditorEventType> originator, int typeValue, int frameIndex, DrawingFrame frame)
            : base(typeValue, originator)
        {
            FrameIndex = frameIndex;
            Frame = frame.Clone(true);
        }

        public override IMemento<IPixelEditorEventType> Restore()
        {
            return _originatorHandler.HandleRestore(this);
        }

        public void Dispose()
        {
            Debug.WriteLine("Disposing of FrameMemento...");
            PixellationCaretakerManager.GetInstance().RemoveCaretaker(Frame.Id, true);
        }
    }
}
