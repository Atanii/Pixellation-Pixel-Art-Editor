using Pixellation.Components.Editor;
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
        public IVisualManager LayerManager
        {
            get { return (IVisualManager)GetValue(LayerListProperty); }
            set { SetValue(LayerListProperty, value); }
        }

        public static readonly DependencyProperty LayerListProperty = DependencyProperty.Register(
             "LayerManager",
             typeof(IVisualManager),
             typeof(Transforms),
             new FrameworkPropertyMetadata()
        );

        public int LayerWidth
        {
            get { return (int)GetValue(LayerWidthProperty); }
            set { SetValue(LayerWidthProperty, value); }
        }

        public static readonly DependencyProperty LayerWidthProperty = DependencyProperty.Register(
             "LayerWidth",
             typeof(int),
             typeof(Transforms),
             new FrameworkPropertyMetadata()
        );

        public int LayerHeight
        {
            get { return (int)GetValue(LayerHeightProperty); }
            set { SetValue(LayerHeightProperty, value); }
        }

        public static readonly DependencyProperty LayerHeightProperty = DependencyProperty.Register(
             "LayerHeight",
             typeof(int),
             typeof(Transforms),
             new FrameworkPropertyMetadata()
        );

        public Transforms()
        {
            InitializeComponent();
        }

        private void MHorizontal_Click(object sender, RoutedEventArgs e)
        {
            var all = (bool)cbAllLayers.IsChecked;
            LayerManager.SaveState(
                all ? IEditorEventType.MIRROR_HORIZONTAL_ALL : IEditorEventType.MIRROR_HORIZONTAL,
                LayerManager.GetActiveLayerIndex()
            );
            LayerManager.Mirror(true, all);
        }

        private void MVertical_Click(object sender, RoutedEventArgs e)
        {
            var all = (bool)cbAllLayers.IsChecked;
            LayerManager.SaveState(
                all ? IEditorEventType.MIRROR_VERTICAL_ALL : IEditorEventType.MIRROR_VERTICAL,
                LayerManager.GetActiveLayerIndex()
            );
            LayerManager.Mirror(false, all);
        }

        private void R90_Click(object sender, RoutedEventArgs e)
        {
            var all = (bool)cbAllLayers.IsChecked;
            var counterClockWise = (bool)cbCounterClockWise.IsChecked;
            if (counterClockWise)
            {
                LayerManager.SaveState(
                    all ? IEditorEventType.ROTATE_COUNTERCLOCKWISE_ALL : IEditorEventType.ROTATE_COUNTERCLOCKWISE,
                    LayerManager.GetActiveLayerIndex()
                );
            }
            else
            {
                LayerManager.SaveState(
                    all ? IEditorEventType.ROTATE_ALL : IEditorEventType.ROTATE,
                    LayerManager.GetActiveLayerIndex()
                );
            }
            LayerManager.Rotate(all, counterClockWise);
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            var w = txtWidth.Text;
            var h = txtHeight.Text;
            if (int.TryParse(w, out int width) && int.TryParse(h, out int height) &&
                width > 0 && height > 0)
            {
                LayerManager.SaveState(
                    IEditorEventType.RESIZE,
                    LayerManager.GetActiveLayerIndex()
                );
                LayerManager.Resize(width, height);
            }
        }

        private void TxtWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((bool)cbResizeProportional.IsChecked && 
                double.TryParse(txtWidth.Text, out double width) && double.TryParse(txtHeight.Text, out double height) &&
                width > 0 && height > 0)
            {
                double prop = width / LayerWidth;
                txtHeight.Text = ((int)(height * prop)).ToString();
            }
        }

        private void TxtHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((bool)cbResizeProportional.IsChecked &&
                double.TryParse(txtHeight.Text, out double height) && double.TryParse(txtWidth.Text, out double width) &&
                height > 0 && width > 0)
            {
                double prop = height / LayerHeight;
                txtWidth.Text = ((int)(width * prop)).ToString();
            }
        }
    }
}