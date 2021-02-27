using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor;
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
        private static event EventHandler<DependencyPropertyChangedEventArgs> RaiseFrameManagerPropertyPropertyInitialized;

        private readonly Thickness FrameElementMargin = new Thickness(12);
        private readonly double FrameElementSize = 100d;

        public IFrameManager FrameManager
        {
            get { return (IFrameManager)GetValue(FrameManagerProperty); }
            set { SetValue(FrameManagerProperty, value); }
        }

        public static readonly DependencyProperty FrameManagerProperty =
         DependencyProperty.Register("FrameManager", typeof(IFrameManager), typeof(FramePalette), new FrameworkPropertyMetadata(
             default,
             (s, e) => { RaiseFrameManagerPropertyPropertyInitialized?.Invoke(s, e); }
        ));

        public FramePalette()
        {
            InitializeComponent();
            RaiseFrameManagerPropertyPropertyInitialized += (s, e) =>
            {
                Refresh();
            };
            PixelEditor.FrameListChanged += (s, e) =>
            {
                Refresh();
            };
        }

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

        private void BtnAddFrame(object sender, RoutedEventArgs e)
        {   
            var newLayerDialog = new StringInputDialog("New Image", "Image Name");
            if (newLayerDialog.ShowDialog() == true)
            {
                FrameManager.AddDrawingFrame(FrameManager.ActiveFrameIndex, newLayerDialog.Answer);
            }
        }

        private void BtnDuplicateFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.DuplicateDrawingFrame(FrameManager.ActiveFrameIndex);
        }

        private void BtnMergeFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.MergeDrawingFrameIntoLeftNeighbour(FrameManager.ActiveFrameIndex);
        }

        private void BtnMoveFrameLeft(object sender, RoutedEventArgs e)
        {
            FrameManager.MoveDrawingFrameLeft(FrameManager.ActiveFrameIndex);
        }

        private void BtnMoveFrameRight(object sender, RoutedEventArgs e)
        {
            FrameManager.MoveDrawingFrameRight(FrameManager.ActiveFrameIndex);
        }

        private void BtnRemoveFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.RemoveDrawingFrame(FrameManager.ActiveFrameIndex);
        }

        private void BtnResetFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.RemoveDrawingFrame(FrameManager.ActiveFrameIndex);
        }
    }
}