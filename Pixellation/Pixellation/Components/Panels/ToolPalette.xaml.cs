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
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly ITool Pencil = PencilTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool Eraser = EraserTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool PaintBucket = PaintBucketTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool Line = LineTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool Dithering = DitheringTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool Pipette = PipetteTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool Circle = EllipseTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool Rectangle = RectangleTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool SelectionRectangle = SelectCopyPasteRectangleTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool SelectionEllipse = SelectCopyPasteEllipseTool.GetInstance(Res.PrimaryToolInstanceKey);
        public static readonly ITool SameColorPaintBucket = SameColorPaintBucketTool.GetInstance(Res.PrimaryToolInstanceKey);

        private readonly Thickness ThicknessClicked = new Thickness() { Top = 1.0, Right = 1.0, Bottom = 1.0, Left = 1.0 };
        private readonly Thickness ThicknessDefault = new Thickness() { Top = 0.0, Right = 0.0, Bottom = 0.0, Left = 0.0 };

        private Button PreviouslyClicked;

        private ITool _chosenTool;

        public ITool ChosenTool
        {
            get => _chosenTool;
            set
            {
                _chosenTool = value;
                OnPropertyChanged();
            }
        }

        public ToolPalette()
        {
            InitializeComponent();

            // Select Pencil by default
            PreviouslyClicked = BtnPencil;
            BtnPencil.BorderThickness = ThicknessClicked;
            ChosenTool = Pencil;
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
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
                "BtnCircle" => Circle,
                "BtnRectangle" => Rectangle,
                "BtnSelection" => SelectionRectangle,
                "BtnEllipseSelection" => SelectionEllipse,
                "BtnSameColorPaintBucket" => SameColorPaintBucket,
                _ => Pencil,
            };
        }

        private void CbEraserMode_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.EraserModeOn = (bool)cbEraserMode.IsChecked;
        }

        private void RbMMNone_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.MirrorMode = MirrorModeStates.OFF;
        }

        private void RbMMHorizontal_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.MirrorMode = MirrorModeStates.HORIZONTAL;
        }

        private void RbMMVertical_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.MirrorMode = MirrorModeStates.VERTICAL;
        }

        private void RbTh1_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.Thickness = ToolThickness.NORMAL;
        }

        private void RbTh2_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.Thickness = ToolThickness.MEDIUM;
        }

        private void RbTh3_Click(object sender, RoutedEventArgs e)
        {
            ChosenTool.Thickness = ToolThickness.WIDE;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}