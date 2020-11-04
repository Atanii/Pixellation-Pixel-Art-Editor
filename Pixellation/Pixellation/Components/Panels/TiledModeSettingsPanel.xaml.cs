using Pixellation.Components.Editor;
using System.Windows;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for TiledModeSettingsPanel.xaml
    /// </summary>
    public partial class TiledModeSettingsPanel : UserControl
    {
        public PixelEditor Editor
        {
            get { return (PixelEditor)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        public static readonly DependencyProperty EditorProperty =
         DependencyProperty.Register("Editor", typeof(PixelEditor), typeof(TiledModeSettingsPanel), new FrameworkPropertyMetadata(default));

        public TiledModeSettingsPanel()
        {
            InitializeComponent();
        }

        private void TiledModeToggle_Click(object sender, RoutedEventArgs e)
        {
            Editor?.ToggleTiled();
        }

        private void TiledModeOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Editor?.SetTiledOpacity(100 - (int)e.NewValue);
        }
    }
}