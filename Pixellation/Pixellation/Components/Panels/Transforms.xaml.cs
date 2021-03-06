using Pixellation.Components.Event;
using Pixellation.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for Transforms.xaml
    /// </summary>
    public partial class Transforms : UserControl
    {
        /// <summary>
        /// Provides layers, functions and indexes for handling layers.
        /// </summary>
        public ILayerManager LayerManager
        {
            get { return (ILayerManager)GetValue(LayerListProperty); }
            set { SetValue(LayerListProperty, value); }
        }

        /// <summary>
        /// DepdendencyProperty for <see cref="LayerManager"/>.
        /// </summary>
        public static readonly DependencyProperty LayerListProperty = DependencyProperty.Register(
             "LayerManager",
             typeof(ILayerManager),
             typeof(Transforms),
             new FrameworkPropertyMetadata()
        );

        /// <summary>
        /// Inits the component.
        /// </summary>
        public Transforms()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Mirrors layer horizontally.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MHorizontal_Click(object sender, RoutedEventArgs e)
        {
            var all = (bool)cbAllLayers.IsChecked;
            LayerManager.SaveState(
                all ? IPixelEditorEventType.MIRROR_HORIZONTAL_ALL : IPixelEditorEventType.MIRROR_HORIZONTAL,
                LayerManager.ActiveLayerIndex
            );
            LayerManager.Mirror(true, all);
        }

        /// <summary>
        /// Mirrors layer vertically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MVertical_Click(object sender, RoutedEventArgs e)
        {
            var all = (bool)cbAllLayers.IsChecked;
            LayerManager.SaveState(
                all ? IPixelEditorEventType.MIRROR_VERTICAL_ALL : IPixelEditorEventType.MIRROR_VERTICAL,
                LayerManager.ActiveLayerIndex
            );
            LayerManager.Mirror(false, all);
        }

        /// <summary>
        /// Rotates image by 90° degrees. (Counter- or clockwise based on the settings.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void R90_Click(object sender, RoutedEventArgs e)
        {
            var counterClockWise = (bool)cbCounterClockWise.IsChecked;
            LayerManager.SaveState(
                counterClockWise ? IPixelEditorEventType.ROTATE_COUNTERCLOCKWISE : IPixelEditorEventType.ROTATE,
                LayerManager.ActiveLayerIndex
            );
            LayerManager.Rotate(counterClockWise);
        }

        /// <summary>
        /// Resizes the image to the given size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            var w = txtWidth.Text;
            var h = txtHeight.Text;
            if (int.TryParse(w, out int width) && int.TryParse(h, out int height) &&
                width > 0 && height > 0)
            {
                LayerManager.SaveState(
                    IPixelEditorEventType.RESIZE,
                    LayerManager.ActiveLayerIndex
                );
                LayerManager.Resize(width, height);
            }
        }

        /// <summary>
        /// Applies proportion to entered width and updates the input if proportional resizes enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((bool)cbResizeProportional.IsChecked &&
                double.TryParse(txtWidth.Text, out double width) && double.TryParse(txtHeight.Text, out double height) &&
                width > 0 && height > 0)
            {
                double prop = width / LayerManager.PixelWidth;
                txtHeight.Text = ((int)(height * prop)).ToString();
            }
        }

        /// <summary>
        /// Applies proportion to entered height and updates the input if proportional resizes enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((bool)cbResizeProportional.IsChecked &&
                double.TryParse(txtHeight.Text, out double height) && double.TryParse(txtWidth.Text, out double width) &&
                height > 0 && width > 0)
            {
                double prop = height / LayerManager.PixelHeight;
                txtWidth.Text = ((int)(width * prop)).ToString();
            }
        }
    }
}