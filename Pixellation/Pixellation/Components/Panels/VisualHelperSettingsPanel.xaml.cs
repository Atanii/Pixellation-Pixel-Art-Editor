using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for VisualHelperSettingsPanel.xaml
    /// </summary>
    public partial class VisualHelperSettingsPanel : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private float _tiledOpacity;

        /// <summary>
        /// Opacity of tiles in tiled-mode.
        /// </summary>
        public float TiledOpacity
        {
            get { return _tiledOpacity; }
            set
            {
                _tiledOpacity = value;
                OnPropertyChanged();
            }
        }

        private bool _tileModeEnabled;

        /// <summary>
        /// Is tiled mode on or not.
        /// </summary>
        public bool TiledModeEnabled
        {
            get { return _tileModeEnabled; }
            set
            {
                _tileModeEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _showBorder;

        /// <summary>
        /// Shows border around edited image.
        /// </summary>
        public bool ShowBorder
        {
            get { return _showBorder; }
            set
            {
                _showBorder = value;
                OnPropertyChanged();
            }
        }

        private bool _showGrid;

        /// <summary>
        /// Shows grid on canvas.
        /// </summary>
        public bool ShowGrid
        {
            get { return _showGrid; }
            set
            {
                _showGrid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Sets event handlers and default values.
        /// </summary>
        public VisualHelperSettingsPanel()
        {
            InitializeComponent();

            TiledOpacity = Properties.Settings.Default.DefaultTiledOpacity;

            ShowBorder = Properties.Settings.Default.DefaultShowBorder;
            ShowGrid = Properties.Settings.Default.DefaultShowGrid;
            TiledModeEnabled = Properties.Settings.Default.DefaultTiledModeEnabled;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}