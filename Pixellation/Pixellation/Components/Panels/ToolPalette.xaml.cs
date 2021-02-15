using Pixellation.Components.Tools;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Pixellation.Components.Panels
{
    /// <summary>
    /// Interaction logic for ToolPalette.xaml
    /// </summary>
    public partial class ToolPalette : UserControl
    {
        public static readonly ITool Pencil = PencilTool.GetInstance();
        public static readonly ITool Eraser = EraserTool.GetInstance();
        public static readonly ITool PaintBucket = PaintBucketTool.GetInstance();
        public static readonly ITool Line = LineTool.GetInstance();
        public static readonly ITool Dithering = DitheringTool.GetInstance();
        public static readonly ITool Pipette = PipetteTool.GetInstance();
        public static readonly ITool Circle = CircleTool.GetInstance();
        public static readonly ITool Rectangle = RectangleTool.GetInstance();
        public static readonly ITool SelectionRectangle = SelectCopyPasteRectangleTool.GetInstance();
        public static readonly ITool SelectionEllipse = SelectCopyPasteEllipseTool.GetInstance();

        private readonly Thickness ThicknessClicked;
        private readonly Thickness ThicknessDefault;

        private Button PreviouslyClicked;

        public ITool ChosenTool
        {
            get { return (ITool)GetValue(ChosenToolProperty); }
            set { SetValue(ChosenToolProperty, value); }
        }
        public static readonly DependencyProperty ChosenToolProperty =
         DependencyProperty.Register("ChosenTool", typeof(ITool), typeof(ToolPalette), new PropertyMetadata(Pencil));

        public ToolPalette()
        {
            ThicknessClicked = new Thickness()
            {
                Top = 1.0,
                Right = 1.0,
                Bottom = 1.0,
                Left = 1.0
            };
            ThicknessDefault = new Thickness()
            {
                Top = 0.0,
                Right = 0.0,
                Bottom = 0.0,
                Left = 0.0
            };

            InitializeComponent();

            // Selected by default
            PreviouslyClicked = BtnPencil;
            BtnPencil.BorderThickness = ThicknessClicked;
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            PreviouslyClicked.BorderThickness = ThicknessDefault;

            var a = (Button)sender;

            a.BorderThickness = ThicknessClicked;
            PreviouslyClicked = a;

            ChosenTool = a.Name switch
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
                _ => Pencil,
            };
        }
    }
}