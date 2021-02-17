using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using static Pixellation.Components.Editor.PixelEditor;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for Transforms.xaml
    /// </summary>
    public partial class Transforms : UserControl
    {
        public VisualManager LayerManager
        {
            get { return (VisualManager)GetValue(LayerListProperty); }
            set { SetValue(LayerListProperty, value); }
        }

        public static readonly DependencyProperty LayerListProperty = DependencyProperty.Register(
             "LayerManager",
             typeof(VisualManager),
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

        private void mHorizontal_Click(object sender, RoutedEventArgs e)
        {
            LayerManager.Mirror(true, (bool)cbAllLayers.IsChecked);
        }

        private void mVertical_Click(object sender, RoutedEventArgs e)
        {
            LayerManager.Mirror(false, (bool)cbAllLayers.IsChecked);
        }

        private void r90_Click(object sender, RoutedEventArgs e)
        {
            LayerManager.Rotate(90, (bool)cbAllLayers.IsChecked, (bool)cbCounterClockWise.IsChecked);
        }

        private void btnResize_Click(object sender, RoutedEventArgs e)
        {
            var w = txtWidth.Text;
            var h = txtHeight.Text;
            if (int.TryParse(w, out int width) && int.TryParse(h, out int height) &&
                width > 0 && height > 0)
            {
                LayerManager.Resize(width, height);
            }
        }

        private void txtWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((bool)cbResizeProportional.IsChecked && 
                double.TryParse(txtWidth.Text, out double width) && double.TryParse(txtHeight.Text, out double height) &&
                width > 0 && height > 0)
            {
                double prop = width / LayerWidth;
                txtHeight.Text = ((int)(height * prop)).ToString();
            }
        }

        private void txtHeight_LostFocus(object sender, RoutedEventArgs e)
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