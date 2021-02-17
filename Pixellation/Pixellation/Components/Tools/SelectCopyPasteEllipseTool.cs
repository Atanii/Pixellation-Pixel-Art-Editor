using Pixellation.Utils;
using System;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Components.Tools
{
    internal class SelectCopyPasteEllipseTool : BaseTool
    {
        private static SelectCopyPasteEllipseTool _instance;

        private Point p0;
        private Point p1;
        private Point p1prev;

        private Color _selectionFillColour;

        private static Rect _selectionArea;
        private static Rect _copyArea;

        private WriteableBitmap _copySrc;

        private bool _creating = false;
        private bool _dragging = false;

        private SelectCopyPasteEllipseTool() : base()
        {
            _selectionFillColour = Color.FromArgb(100, 100, 100, 100);
        }

        public static SelectCopyPasteEllipseTool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SelectCopyPasteEllipseTool();
            }

            return _instance;
        }

        public override void OnMouseDown(MouseButtonEventArgs e)
        {
            p0 = e.GetPosition(_previewLayer);
            p1prev = p0;
            p0 = p0.IntDivide(_magnification);

            if (!_selectionArea.Contains(p0))
            {
                if (_copySrc != null)
                {
                    _copySrc.Clear();
                }

                _previewLayer.GetWriteableBitmap().Clear();

                _selectionArea.X = p0.X;
                _selectionArea.Y = p0.Y;
                _selectionArea.Width = 0;
                _selectionArea.Height = 0;

                _creating = true;
                _dragging = false;
            }
            else
            {
                _dragging = true;
                _creating = false;
            }
        }

        public override void OnMouseUp(MouseButtonEventArgs e)
        {
            _creating = false;
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                p1 = e.GetPosition(_previewLayer);

                if (_creating)
                {
                    p1 = p1.IntDivide(_magnification);

                    var diff = (p1 - p0);

                    if (diff.X < 0)
                    {
                        _selectionArea.X = p1.X;
                    }
                    if (diff.Y < 0)
                    {
                        _selectionArea.Y = p1.Y;
                    }

                    _selectionArea.Width = Math.Abs(diff.X);
                    _selectionArea.Height = Math.Abs(diff.Y);

                    var x1 = (int)_selectionArea.X;
                    var y1 = (int)_selectionArea.Y;
                    var x2 = (int)(_selectionArea.X + _selectionArea.Width);
                    var y2 = (int)(_selectionArea.Y + _selectionArea.Height);

                    _previewLayer.GetWriteableBitmap().Clear();
                    _previewLayer.GetWriteableBitmap().FillEllipse(
                        x1, y1, x2, y2, _selectionFillColour
                    );
                }

                if (_dragging && !_creating)
                {
                    var diff = p1 - p1prev;

                    var tmpX = _selectionArea.X + (Math.Ceiling(diff.X) / _magnification);
                    var tmpY = _selectionArea.Y + (Math.Ceiling(diff.Y) / _magnification);
                    var tmpX2 = tmpX + _selectionArea.Width;
                    var tmpY2 = tmpY + _selectionArea.Height;

                    if (tmpX < 0)
                    {
                        tmpX = 0;
                        tmpX2 = tmpX + _selectionArea.Width;
                    }
                    else if (tmpX2 >= _previewLayer.ActualWidth / _magnification)
                    {
                        tmpX2 = _previewLayer.ActualWidth / _magnification;
                        tmpX = tmpX2 - _selectionArea.Width;
                    }

                    if (tmpY < 0)
                    {
                        tmpY = 0;
                        tmpY2 = tmpY + _selectionArea.Height;
                    }
                    else if (tmpY2 >= _previewLayer.ActualHeight / _magnification)
                    {
                        tmpY2 = _previewLayer.ActualHeight / _magnification;
                        tmpY = tmpY2 - _selectionArea.Height;
                    }

                    _selectionArea.X = tmpX;
                    _selectionArea.Y = tmpY;

                    var x1 = (int)_selectionArea.X;
                    var y1 = (int)_selectionArea.Y;
                    var x2 = (int)tmpX2;
                    var y2 = (int)tmpY2;

                    _previewLayer.GetWriteableBitmap().Clear();
                    _previewLayer.GetWriteableBitmap().FillEllipse(
                        x1, y1, x2, y2, _selectionFillColour
                    );
                }

                p1prev = p1;
            }
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.X)
            {
                _copyArea.X = _selectionArea.X;
                _copyArea.Y = _selectionArea.Y;
                _copyArea.Width = _selectionArea.Width;
                _copyArea.Height = _selectionArea.Height;
                _copySrc = _layer.GetWriteableBitmap().Clone();

                var x1 = (int)_selectionArea.X;
                var y1 = (int)_selectionArea.Y;
                var x2 = (int)(_selectionArea.X + _selectionArea.Width);
                var y2 = (int)(_selectionArea.Y + _selectionArea.Height);

                System.Drawing.Rectangle boundingBox = new System.Drawing.Rectangle(x1, y1, x2, y2);
                GraphicsPath ellipse = new GraphicsPath();
                ellipse.AddEllipse(boundingBox);
                _copySrc.ClearPixelsByGraphicsPath(ellipse, false);

                _layer.GetWriteableBitmap().FillEllipse(
                        x1, y1, x2, y2, Colors.Transparent
                );
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.C)
            {
                _copyArea.X = _selectionArea.X;
                _copyArea.Y = _selectionArea.Y;
                _copyArea.Width = _selectionArea.Width;
                _copyArea.Height = _selectionArea.Height;
                _copySrc = _layer.GetWriteableBitmap().Clone();
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
            {
                _layer.GetWriteableBitmap().Blit(_selectionArea, _copySrc, _copyArea, WriteableBitmapExtensions.BlendMode.Alpha);
            }
        }

        public override void Reset()
        {
            _selectionArea.X = 0;
            _selectionArea.Y = 0;
            _selectionArea.Width = 0;
            _selectionArea.Height = 0;

            _copyArea.X = 0;
            _copyArea.Y = 0;
            _copyArea.Width = 0;
            _copyArea.Height = 0;

            if (_copySrc != null)
            {
                _copySrc.Clear();
            }

            _previewLayer.GetWriteableBitmap().Clear();
        }
    }
}