using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for EditorSettingsPanel.xaml
    /// </summary>
    public partial class EditorSettingsPanel : UserControl, INotifyPropertyChanged
    {
        private float _tiledOpacity;
        public float TiledOpacity {
            get { return _tiledOpacity; }
            set {
                _tiledOpacity = value;
                OnPropertyChanged();
            }
        }

        private bool _tileModeOn;
        public bool TiledModeOn
        {
            get { return _tileModeOn; }
            set
            {
                _tileModeOn = value;
                OnPropertyChanged();
            }
        }

        private bool _showBorderOn;
        public bool ShowBorderOn
        {
            get { return _showBorderOn; }
            set
            {
                _showBorderOn = value;
                OnPropertyChanged();
            }
        }

        private bool _showGridOn;
        public bool ShowGridOn
        {
            get { return _showGridOn; }
            set
            {
                _showGridOn = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EditorSettingsPanel()
        {
            InitializeComponent();

            tiledModeOpacitySlider.Value = Properties.Settings.Default.DefaultTiledOpacity * 100d;

            ShowBorderOn = Properties.Settings.Default.DefaultShowBorder;
            ShowGridOn = Properties.Settings.Default.DefaultShowGrid;
            TiledModeOn = Properties.Settings.Default.DefaultTiledModeOn;
        }

        private void TiledModeOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TiledOpacity = ((float)e.NewValue / 100.0f);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}