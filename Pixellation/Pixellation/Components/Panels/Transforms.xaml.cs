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

        public Transforms()
        {
            InitializeComponent();

            /*txtWidth.Text = LayerManager.GetSize().Width.ToString();
            txtHeight.Text = LayerManager.GetSize().Height.ToString();*/
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

        private void txtWidth_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var w = txtWidth.Text;
            if ((bool)cbResizeProportional.IsChecked && 
                double.TryParse(w, out double width) &&
                width > 0)
            {
                double prop = width / LayerManager.GetSize().Width;
                txtHeight.Text = ((int)(LayerManager.GetSize().Height * prop)).ToString();
            }
        }

        private void txtHeight_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var h = txtHeight.Text;
            if ((bool)cbResizeProportional.IsChecked &&
                double.TryParse(h, out double height) &&
                height > 0)
            {
                double prop = height / LayerManager.GetSize().Height;
                txtWidth.Text = ((int)(LayerManager.GetSize().Width * prop)).ToString();
            }
        }
    }
}