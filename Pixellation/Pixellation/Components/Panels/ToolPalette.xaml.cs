using Pixellation.Tools;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    using Res = Properties.Resources;

    /// <summary>
    /// Interaction logic for ToolPalette.xaml
    /// </summary>
    public partial class ToolPalette : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Event used for one- and twoway databinding.
        /// Marks change regarding one of the properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Pencil drawing tool.
        /// </summary>
        public static readonly ITool Pencil = PencilTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Eraser drawing tool.
        /// </summary>
        public static readonly ITool Eraser = EraserTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Paintbucket drawing tool.
        /// </summary>
        public static readonly ITool PaintBucket = PaintBucketTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Line drawing tool.
        /// </summary>
        public static readonly ITool Line = LineTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Dithering tool for drawing "chessboard" pattern.
        /// </summary>
        public static readonly ITool Dithering = DitheringTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Pipette colorpicking tool.
        /// </summary>
        public static readonly ITool Pipette = PipetteTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Ellipse drawing tool.
        /// </summary>
        public static readonly ITool Ellipse = EllipseTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Rectangle drawing tool.
        /// </summary>
        public static readonly ITool Rectangle = RectangleTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Tool for select-copy-cut-paste within a rectangle.
        /// </summary>
        public static readonly ITool SelectionRectangle = SelectCopyPasteRectangleTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Tool for select-copy-cut-paste within a ellipse.
        /// </summary>
        public static readonly ITool SelectionEllipse = SelectCopyPasteEllipseTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Colors every pixel with target color to one another color.
        /// </summary>
        public static readonly ITool SameColorPaintBucket = SameColorPaintBucketTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Tool for lightening and darkening pixels.
        /// </summary>
        public static readonly ITool DarkenLighten = ShadingTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Tool for balancing pixels. With left click it decrease the maximum from the RGB of the clicked pixel, on right click the opposite.
        /// </summary>
        public static readonly ITool Balancer = BalancerTool.GetInstance(Res.PrimaryToolInstanceKey);

        /// <summary>
        /// Borderthickness for the icon of clicked tool.
        /// </summary>
        private readonly Thickness ThicknessClicked = new Thickness() { Top = 1.0, Right = 1.0, Bottom = 1.0, Left = 1.0 };

        /// <summary>
        /// Default borderthickness for icons of tools.
        /// </summary>
        private readonly Thickness ThicknessDefault = new Thickness() { Top = 0.0, Right = 0.0, Bottom = 0.0, Left = 0.0 };

        /// <summary>
        /// Button of last clicked tool.
        /// </summary>
        private Button PreviouslyClicked;

        private ITool _chosenTool;

        /// <summary>
        /// Currently active tool.
        /// </summary>
        public ITool ChosenTool
        {
            get => _chosenTool;
            set
            {
                _chosenTool = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Inits default values.
        /// </summary>
        public ToolPalette()
        {
            InitializeComponent();

            // Select Pencil by default.
            PreviouslyClicked = BtnPencil;
            BtnPencil.BorderThickness = ThicknessClicked;
            ChosenTool = Pencil;
            // Set tool settings to default.
            ResetModes();
        }

        /// <summary>
        /// Selects the clicked button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            ResetModes();

            PreviouslyClicked.BorderThickness = ThicknessDefault;

            var btn = (Button)sender;
            btn.BorderThickness = ThicknessClicked;

            switch (PreviouslyClicked.Name)
            {
                case "BtnSelection":
                    SelectionRectangle.Reset();
                    break;

                case "BtnEllipseSelection":
                    SelectionEllipse.Reset();
                    break;
            }

            PreviouslyClicked = btn;
            ChosenTool = btn.Name switch
            {
                "BtnPencil" => Pencil,
                "BtnEraser" => Eraser,
                "BtnPaintBucket" => PaintBucket,
                "BtnLine" => Line,
                "BtnDithering" => Dithering,
                "BtnPipette" => Pipette,
                "BtnCircle" => Ellipse,
                "BtnRectangle" => Rectangle,
                "BtnSelection" => SelectionRectangle,
                "BtnEllipseSelection" => SelectionEllipse,
                "BtnSameColorPaintBucket" => SameColorPaintBucket,
                "BtnLightenDarken" => DarkenLighten,
                "BtnBalancer" => Balancer,
                _ => Pencil,
            };
        }

        /// <summary>
        /// Reset different tool options and UI.
        /// </summary>
        private void ResetModes()
        {
            ChosenTool.EraserModeOn = false;
            ChosenTool.MirrorMode = MirrorModeStates.OFF;
            ChosenTool.Thickness = ToolThickness.NORMAL;

            cbEraserMode.IsChecked = false;
            rbMMNone.IsChecked = true;
            rbTh1.IsChecked = true;
        }

        /// <summary>
        /// Turns mirror mode off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbMMNone_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.MirrorMode = MirrorModeStates.OFF;
        }

        /// <summary>
        /// Turns mirror mode to horizontal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbMMHorizontal_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.MirrorMode = MirrorModeStates.HORIZONTAL;
        }

        /// <summary>
        /// Turns mirror mode to vertical.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbMMVertical_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.MirrorMode = MirrorModeStates.VERTICAL;
        }

        /// <summary>
        /// Sets line thickness to the normal..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbTh1_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.Thickness = ToolThickness.NORMAL;
        }

        /// <summary>
        /// Sets line thickness to medium.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbTh2_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.Thickness = ToolThickness.MEDIUM;
        }

        /// <summary>
        /// Sets line thickness to wide.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbTh3_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.Thickness = ToolThickness.WIDE;
        }

        /// <summary>
        /// Used as change notification for one- and twoway binding with <see cref="DependencyProperty"/> objects.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SetEraserMode(object sender, RoutedEventArgs e)
        {
            ChosenTool.EraserModeOn = (bool)cbEraserMode.IsChecked;
        }
    }
}