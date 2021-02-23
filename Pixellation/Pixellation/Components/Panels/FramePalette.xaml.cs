using Pixellation.Components.Dialogs.StringInputDialog;
using Pixellation.Components.Editor;
using Pixellation.Interfaces;
using System;
using System.Diagnostics;
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
            foreach (var frame in FrameManager.GetFrames())
            {
                frame.Width = FrameElementSize;
                frame.Height = FrameElementSize;
                frame.Margin = FrameElementMargin;
                frameList.Children.Add(frame);
            }
            InvalidateVisual();
            InvalidateMeasure();
            InvalidateArrange();
        }

        private void BtnAddFrame(object sender, RoutedEventArgs e)
        {   
            var newLayerDialog = new StringInputDialog("New Image", "Image Name");
            if (newLayerDialog.ShowDialog() == true)
            {
                FrameManager.AddDrawingFrame(FrameManager.GetActiveFrameIndex(), newLayerDialog.Answer);
            }
        }

        private void BtnDuplicateFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.DuplicateDrawingFrame(FrameManager.GetActiveFrameIndex());
        }

        private void BtnMergeFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.MergeDrawingFrameIntoLeftNeighbour(FrameManager.GetActiveFrameIndex());
        }

        private void BtnRemoveFrame(object sender, RoutedEventArgs e)
        {
            FrameManager.RemoveDrawingFrame(FrameManager.GetActiveFrameIndex());
        }

        private void BtnMoveFrameLeft(object sender, RoutedEventArgs e)
        {
            FrameManager.MoveDrawingFrameLeft(FrameManager.GetActiveFrameIndex());
        }

        private void BtnMoveFrameRight(object sender, RoutedEventArgs e)
        {
            FrameManager.MoveDrawingFrameRight(FrameManager.GetActiveFrameIndex());
        }
    }
}