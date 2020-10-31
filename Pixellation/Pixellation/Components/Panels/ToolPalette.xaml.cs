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

        public ITool ChosenTool
        {
            get { return (ITool)GetValue(ChosenToolProperty); }
            set { SetValue(ChosenToolProperty, value); }
        }
        public static readonly DependencyProperty ChosenToolProperty =
         DependencyProperty.Register("ChosenTool", typeof(ITool), typeof(ToolPalette), new PropertyMetadata(Pencil));

        public ToolPalette()
        {
            InitializeComponent();
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var a = (Button)sender;
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
                _ => Pencil,
            };
        }
    }
}