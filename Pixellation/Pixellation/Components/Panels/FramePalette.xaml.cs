using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor;
using Pixellation.Components.Event;
using Pixellation.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for FramePalette.xaml
    /// </summary>
    public partial class FramePalette : UserControl
    {
        /// <summary>
        /// Event for signaling the initial or new value set to <see cref="FrameManager"/>.
        /// </summary>
        private static event EventHandler<DependencyPropertyChangedEventArgs> RaiseFrameManagerPropertyPropertyInitialized;

        /// <summary>
        /// Margin for frame child <see cref="Visual"/>.
        /// </summary>
        private readonly Thickness FrameElementMargin = new Thickness(12);

        /// <summary>
        /// Size for frame child <see cref="Visual"/>.
        /// </summary>
        private readonly double FrameElementSize = 100d;

        /// <summary>
        /// Provides frames, indexes and functionality for handling frames.
        /// </summary>
        public IFrameManager FrameManager
        {
            get { return (IFrameManager)GetValue(FrameManagerProperty); }
            set { SetValue(FrameManagerProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="FrameManager"/>.
        /// </summary>
        public static readonly DependencyProperty FrameManagerProperty =
         DependencyProperty.Register("FrameManager", typeof(IFrameManager), typeof(FramePalette), new FrameworkPropertyMetadata(
             default,
             (s, e) => { RaiseFrameManagerPropertyPropertyInitialized?.Invoke(s, e); }
        ));

        /// <summary>
        /// Inits event handling.
        /// </summary>
        public FramePalette()
        {
            InitializeComponent();
            // Image update -> frame render update
            RaiseFrameManagerPropertyPropertyInitialized += (s, e) => { Refresh(); };
            // Frames merged, moved, ...
            PixelEditor.FrameListChanged += (e) => { Refresh(); };
            // Name changed, ...
            DrawingFrame.PropertyUpdated += Refresh;
        }
        
        /// <summary>
        /// Refresh the drawn list of frames.
        /// </summary>
        private void Refresh()
        {
            frameList.Children.Clear();
            foreach (var frame in FrameManager.Frames)
            {
                frame.Width = FrameElementSize;
                frame.Height = FrameElementSize;
                frame.Margin = FrameElementMargin;

                frameList.Children.Add(frame);
            }
            InvalidateVisual();
        }

        /// <summary>
        /// Ask for a name then adds a new frame with that name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddFrame(object sender, RoutedEventArgs e)
        {   
            var newLayerDialog = new StringInputDialog("New Image", "Image Name");
            if (newLayerDialog.ShowDialog() == true)
            {
                FrameManager.AddDrawingFrame(FrameManager.ActiveFrameIndex, newLayerDialog.Answer);
                FrameManager.SaveState(IPixelEditorEventType.FRAME_ADD, FrameManager.ActiveFrameIndex);
            }
        }

        /// <summary>
        /// Duplicates selected frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDuplicateFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.DuplicateDrawingFrame(FrameManager.ActiveFrameIndex);
            FrameManager.SaveState(IPixelEditorEventType.FRAME_DUPLICATE, FrameManager.ActiveFrameIndex);
        }

        /// <summary>
        /// Merges selected frame into the left neighbour. (Irreversible!)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMergeFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.MergeDrawingFrameIntoLeftNeighbour(FrameManager.ActiveFrameIndex);
        }

        /// <summary>
        /// Moves selected frame left (behind).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMoveFrameLeft(object sender, RoutedEventArgs e)
        {
            FrameManager.SaveState(IPixelEditorEventType.FRAME_MOVE_LEFT, FrameManager.ActiveFrameIndex);
            FrameManager.MoveDrawingFrameLeft(FrameManager.ActiveFrameIndex);
        }

        /// <summary>
        /// Moves selected frame right (closer).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMoveFrameRight(object sender, RoutedEventArgs e)
        {
            FrameManager.SaveState(IPixelEditorEventType.FRAME_MOVE_RIGHT, FrameManager.ActiveFrameIndex);
            FrameManager.MoveDrawingFrameRight(FrameManager.ActiveFrameIndex);
        }

        /// <summary>
        /// Removes selected frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoveFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.SaveState(IPixelEditorEventType.FRAME_REMOVE, FrameManager.ActiveFrameIndex);
            FrameManager.RemoveDrawingFrame(FrameManager.ActiveFrameIndex);
        }

        /// <summary>
        /// Resets selected frame. (Irreversible!)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.ResetDrawingFrame(FrameManager.ActiveFrameIndex);
        }

        /// <summary>
        /// Undo last saved undoable frame operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUndoFrameOperation(object sender, RoutedEventArgs e)
        {
            FrameManager.UndoFrameOperation();
        }

        /// <summary>
        /// Redo last saved undone frame operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRedoFrameOperation(object sender, RoutedEventArgs e)
        {
            FrameManager.RedoFrameOperation();
        }
    }
}