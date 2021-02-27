using Pixellation.Components.Editor;
using System.Drawing;
using System.Windows.Input;
using static Pixellation.Tools.BaseTool;

namespace Pixellation.Tools
{
    public interface ITool
    {
        public void SetAllDrawingCircumstances(int magnification, int pixelWidth, int pixelHeight, DrawingLayer ds, DrawingLayer previewLayer, MirrorModeStates mirrorModeState = MirrorModeStates.OFF, ToolThickness thickness = ToolThickness.NORMAL);

        public void SetPixelSize(int pixelWidth, int pixelHeight);

        public void SetMagnification(int magnification);

        public void SetActiveLayer(DrawingLayer layer);

        public void SetPreviewLayer(DrawingLayer layer);

        public void SetDrawColor(Color c);

        public Color GetDrawColor();

        public void SetMirrorMode(MirrorModeStates mirrorMode);

        public void OnMouseMove(MouseEventArgs e);

        public void OnMouseDown(MouseButtonEventArgs e);

        public void OnMouseUp(MouseButtonEventArgs e);

        public void OnKeyDown(KeyEventArgs e);

        public void Reset();

        public ITool GetInstanceByKey(string key);

        public System.Windows.Media.Color ToolColor { get; set; }

        public MirrorModeStates MirrorMode { get; set; }

        public ToolThickness Thickness { get; set; }

        public bool EraserModeOn { get; set; }
    }
}