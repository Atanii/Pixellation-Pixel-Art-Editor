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
    }
}