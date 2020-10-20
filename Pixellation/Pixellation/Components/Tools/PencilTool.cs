﻿using System.Windows.Input;

namespace Pixellation.Components.Tools
{
    public class PencilTool : BaseTool
    {
        private static PencilTool _instance;

        private PencilTool() : base()
        {
        }

        public static PencilTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PencilTool();
            }
            return _instance;
        }

        private void Draw()
        {
            var p = Mouse.GetPosition(_layer);

            if (p.X < 0 || p.X >= _surfaceWidth || p.Y < 0 || p.Y >= _surfaceHeight)
                return;

            _layer.SetColor(
                (int)(p.X / _magnification),
                (int)(p.Y / _magnification),
                ToolColor);

            _layer.InvalidateVisual();
        }

        public override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Draw();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Draw();
        }
    }
}