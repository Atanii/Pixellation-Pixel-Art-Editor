using Pixellation.Models;
using Pixellation.Utils;
using System;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pixellation.Tools
{
    internal class SelectCopyPasteEllipseTool : BaseTool
    {
        private static SelectCopyPasteEllipseTool _instance;

        private IntPoint p0;
        private IntPoint p1;
        private IntPoint p1prev;

        private Color _selectionFillColour;

        private static IntRect _selectionArea = new IntRect();
        private static IntRect _copyArea = new IntRect();

        private WriteableBitmap _copySrc;

        private bool _creating = false;
        private bool _dragging = false;
        private bool _click = false;

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
            p0 = e.GetPosition(_previewLayer).DivideByIntAsIntPoint(_magnification);
            p1prev = p0;

            if (!_selectionArea.Contains(p0))
            {
                if (_copySrc != null)
                {
                    _copySrc.Clear();
                }

                _selectionArea.X = p0.X;
                _selectionArea.Y = p0.Y;
                _selectionArea.Width = 0;
                _selectionArea.Height = 0;

                _creating = true;
                _dragging = false;
                _click = true;
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
            if (_click)
            {
                _dragging = false;
                Reset();
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _click = false;

                p1 = e.GetPosition(_previewLayer).DivideByIntAsIntPoint(_magnification);

                if (_creating)
                {
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

                    var x1 = _selectionArea.X;
                    var y1 = _selectionArea.Y;
                    var x2 = _selectionArea.Right;
                    var y2 = _selectionArea.Bottom;

                    _previewDrawSurface.Clear();
                    _previewDrawSurface.FillEllipse(
                        x1, y1, x2, y2, _selectionFillColour
                    );
                }

                if (_dragging && !_creating)
                {
                    var diff = p1 - p1prev;

                    var tmpX = _selectionArea.X + diff.X;
                    var tmpY = _selectionArea.Y + diff.Y;
                    var tmpX2 = tmpX + _selectionArea.Width;
                    var tmpY2 = tmpY + _selectionArea.Height;

                    if (tmpX < 0)
                    {
                        tmpX = 0;
                        tmpX2 = tmpX + _selectionArea.Width;
                    }
                    else if (tmpX2 >= _surfaceWidth)
                    {
                        tmpX2 = _surfaceWidth;
                        tmpX = tmpX2 - _selectionArea.Width;
                    }

                    if (tmpY < 0)
                    {
                        tmpY = 0;
                        tmpY2 = tmpY + _selectionArea.Height;
                    }
                    else if (tmpY2 >= _surfaceHeight)
                    {
                        tmpY2 = _surfaceHeight;
                        tmpY = tmpY2 - _selectionArea.Height;
                    }

                    _selectionArea.X = tmpX;
                    _selectionArea.Y = tmpY;

                    var x1 = _selectionArea.X;
                    var y1 = _selectionArea.Y;
                    var x2 = tmpX2;
                    var y2 = tmpY2;

                    _previewDrawSurface.Clear();
                    _previewDrawSurface.FillEllipse(
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
                _copySrc = _drawSurface.Clone();

                var x1 = _selectionArea.X;
                var y1 = _selectionArea.Y;
                var x2 = _selectionArea.Right;
                var y2 = _selectionArea.Bottom;

                System.Drawing.Rectangle boundingBox = new System.Drawing.Rectangle(x1, y1, x2, y2);
                GraphicsPath ellipse = new GraphicsPath();
                ellipse.AddEllipse(boundingBox);
                _copySrc.ClearPixelsByGraphicsPath(ellipse, false);

                _drawSurface.FillEllipse(
                        x1, y1, x2, y2, Colors.Transparent
                );
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.C)
            {
                _copyArea.X = _selectionArea.X;
                _copyArea.Y = _selectionArea.Y;
                _copyArea.Width = _selectionArea.Width;
                _copyArea.Height = _selectionArea.Height;
                _copySrc = _drawSurface.Clone();
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.V)
            {
                var dest = new Rect(_selectionArea.X, _selectionArea.Y, _selectionArea.Width, _selectionArea.Y);
                var cpy = new Rect(_copyArea.X, _copyArea.Y, _copyArea.Width, _copyArea.Y);
                _drawSurface.Blit(dest, _copySrc, cpy, WriteableBitmapExtensions.BlendMode.Alpha);
            }
        }

        public override void Reset()
        {
            _selectionArea = new IntRect();

            _copyArea = new IntRect();

            if (_copySrc != null)
            {
                _copySrc.Clear();
            }

            _previewDrawSurface.Clear();
        }
    }
}