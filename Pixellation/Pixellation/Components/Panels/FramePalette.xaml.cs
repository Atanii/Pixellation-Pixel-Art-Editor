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
                //PixelEditor.RaiseImageUpdatedEvent += (s, e) =>
                //{
                //    Refresh();
                //};
                PixelEditor.FrameListChanged += (s, e) =>
                {
                    Refresh();
                };
            };
        }

        private void Refresh()
        {
            frameList.Children.Clear();
            foreach (var frame in FrameManager.GetFrames())
            {
                frame.Width = 100;
                frame.Height = 100;
                frame.Margin = new Thickness(12);
                frame.IsEnabled = true;
                //frame.MouseDown += frame.OnMouseDown;
                frameList.Children.Add(frame);
            }
            InvalidateVisual();
            InvalidateMeasure();
            InvalidateArrange();
        }

        private void BtnAddFrame(object sender, RoutedEventArgs e)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            FrameManager.AddDrawingFrame(FrameManager.GetActiveFrameIndex(), rng.Next(-1000, 1000).ToString());
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